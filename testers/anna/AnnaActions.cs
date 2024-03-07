using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Combat {
    public partial class Anna {

        public override List<CombatAction> ActionList => FetchActionsFrom(Actions);

        public ActionStore Actions;
        public class ActionStore {
            public ActionClasses.Kick Kick;
            public ActionClasses.Aim Aim;
            public ActionClasses.Shoot Shoot;
            public ActionClasses.Reload Reload;
            public ActionClasses.Smoke Smoke;

            public CommonActions.Move Move;
            public CommonActions.Switch Switch;
            public CommonActions.Pass Pass;

            public ActionStore (Anna anna) {
                foreach (var field in typeof(ActionStore).GetFields()) {
                    field.SetValue(this, Activator.CreateInstance(field.FieldType, anna));
                }
            }
        }

        public class ActionClasses {
            public class Kick : CombatAction {
                public override string Name => "Kick";
                public override int TempoCost { get; set; } = 2;
                public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                    new (TargetType.Single) { Side = SideSelector.Opposite, Row = 0, },
                };
                public override bool IsAvailable () {
                    return User.Row == 0;
                }

                public new Anna User => base.User as Anna;
                public Kick (Anna user) : base (user) {}

                public BasicAttackOptions AttackOptions { get; protected set; } = new () {
                    HitRollTags = new [] { "Attack", "Melee", "Unarmed" },
                    ParryNegation = 2,
                    DodgeNegation = 4,
                };

                public override async Task Run () {
                    var target = Targets[0];

                    User.Animator.Play(User.Animations.Kick);
                    await User.DisplaceToMeleeDistance(target.Combatant);

                    await User.BasicAttack(target, AttackOptions, async result => {
                        if (result.Dodged) {
                            result.AllowRiposte = true;
                        }

                        if (result.Hit) {
                            var damage = User.Roll(10, "Damage", "Melee", "Unarmed");
                            target.Combatant.Damage(damage, new string [] { "Melee", "Unarmed" });
                        }

                        if (User.Bullets > 0 && (result.Hit || result.Parried)) {
                            await InteractionManager.QuickAct(User.Actions.Shoot.Bind(target));
                        }
                    });
                }
            }

            public class Aim : CombatAction {
                public override string Name => "Aim";

                public override int TempoCost { get; set; } = 1;

                public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                    new (TargetType.Single) { Side = SideSelector.Opposite, }
                };

                public override bool IsAvailable () {
                    return User.Row != 0;
                }

                public new Anna User => base.User as Anna;
                public Aim (Anna user) : base (user) {}

                public override async Task Run() {
                    foreach (var combatant in Battle.Combatants) {
                        combatant.RemoveStatusEffectIf<LockedOn>(effect => effect.Caster == User);
                    }

                    Targets[0].Combatant.AddStatusEffect(new LockedOn(User));

                    CombatEvents.BeforeTurnEnd.Once(() => {
                        Targets[0].Combatant.RemoveStatusEffect<LockedOn>();
                    });
                }

                public class LockedOn : StatusEffect {
                    public override string Name => "Locked-On";
                    public Anna Caster;

                    public LockedOn (Anna caster) {
                        Caster = caster;
                    }
                }
            }

            public class Shoot : CombatAction {
                public override string Name => "Shoot";
                public override int TempoCost { get; set; } = 1;

                public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                    new (TargetType.Single) { Side = SideSelector.Opposite, },
                };

                public override bool IsAvailable() {
                    return User.Bullets > 0;
                }

                public new Anna User => base.User as Anna;
                public Shoot (Anna user) : base (user) {}

                public BasicAttackOptions AttackOptions { get; protected set; } = new () {
                    HitRollTags = new string [] { "Attack", "Shot" },
                    ParryNegation = 10,
                    DodgeNegation = 6,
                };

                public override async Task Run () {
                    var target = Targets[0];

                    User.Animator.Play(User.Animations.Shoot);
                    User.Bullets -= 1;

                    var locked_on_modifier = new RollModifier (User.Actions.Aim, "Attack") { Advantage = 1 };;
                    var locked_on = target.Combatant.GetStatusEffect<Aim.LockedOn>() as Aim.LockedOn;
                    if (locked_on?.Caster == User) User.AddRollModifier(locked_on_modifier);

                    await User.BasicAttack(target, AttackOptions, async result => {
                        if (result.Hit) {
                            var damage_roll = User.Roll(6, new string [] { "Damage", "Shot" });
                            target.Combatant.Damage(damage_roll, new string [] { "Bullet" });
                        }
                    });

                    User.TryRemoveRollModifier(locked_on_modifier);
                }
            }
        
            public class Reload : CombatAction {
                public override string Name => "Reload";

                public override int TempoCost { get; set; } = 1;
                public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {};

                public new Anna User => base.User as Anna;
                public Reload (Anna user) : base (user) {}

                public int Amount { get; set; } = 3;

                public override async Task Run () {
                    User.Animator.Play(User.Animations.Reload);
                    User.Bullets += Amount;
                    if (User.Bullets > User.MaxBullets) User.Bullets = User.MaxBullets;
                }
            }
        
            public class Smoke : CombatAction {
                public override string Name => "Smoke";
                public override int TempoCost { get; set; } = 1;

                public override bool IsAvailable () => User.HasStatusEffect<TheShakes>();

                public new Anna User => base.User as Anna;
                public Smoke (Anna user) : base (user) {}

                public override async Task Run () {
                    User.Play(User.Animations.Smoke);
                    var effect = User.GetStatusEffect<TheShakes>();
                    effect.Level -= 2;
                    if (effect.Level < 1) User.RemoveStatusEffect(effect);
                }
            }
        }
    }
}