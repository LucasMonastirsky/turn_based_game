using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Combat {
    public partial class Anna {

        public override List<CombatAction> ActionList => FetchActionsFrom(Actions);

        public ActionStore Actions;
        public class ActionStore {
            public ActionClasses.Kick Kick;
            public ActionClasses.Aim Aim;
            public ActionClasses.Shoot Shoot;
            public ActionClasses.LegShot LegShot;
            public ActionClasses.Reload Reload;
            public ActionClasses.Smoke Smoke;
            public ActionClasses.Unload Unload;

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
                    CommonTargetSelectors.Melee,
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

                    var result = await User.Attack(target, AttackOptions, async result => {
                        if (result.Hit) {
                            var damage = User.Roll(10, "Damage", "Melee", "Unarmed");
                            target.Combatant.Damage(damage, new string [] { "Melee", "Unarmed" });
                        }
                    });

                    if (!result.Dodged && User.Bullets > 0) {
                        await Timing.Delay();
                        await User.Actions.Shoot.Act(target);
                    }
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

                public override async Task Run() { // TODO: implement event to dynamically add mods to attacks
                    User.Play(User.Animations.Shoot);
                    User.Play(User.Sounds.Cock);

                    foreach (var combatant in Battle.Combatants) {
                        combatant.RemoveStatusEffectIf<LockedOn>(effect => effect.Caster == User);
                    }

                    var enemy = Targets[0].Combatant;

                    enemy.AddStatusEffect(new LockedOn(User));

                    CombatEvents.BeforeTurnEnd.Once(async () => {
                        enemy.RemoveStatusEffect<LockedOn>();
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
                    DodgeNegation = 3,
                };

                public override async Task Run () {
                    var target = Targets[0];

                    User.Animator.Play(User.Animations.Shoot);
                    User.Bullets -= 1;

                    var locked_on_modifier = new RollModifier (User.Actions.Aim, "Attack") { Advantage = 1 }; // TODO: do this properly
                    var locked_on = target.Combatant.GetStatusEffect<Aim.LockedOn>() as Aim.LockedOn;
                    if (locked_on?.Caster == User) User.AddRollModifier(locked_on_modifier);

                    await User.Attack(target, AttackOptions, async result => {
                        User.Play(User.Sounds.Shot);

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
                    
                    var step_count = User.MaxBullets - User.Bullets + 2;
                    if (step_count > Amount + 2) step_count = Amount + 2;

                    for (var i = 0; i < step_count; i++) {
                        if (i == 0 || i == step_count - 1) User.Play(User.Sounds.ReloadStart);
                        else User.Play(User.Sounds.ReloadShell);

                        if (i < step_count - 1) await Timing.Delay((float) 1/step_count);
                    }

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
        
            public class LegShot : CombatAction {
                public override string Name => "Leg-Shot";
                public override int TempoCost { get; set; } = 1;

                public override bool IsAvailable () => User.Bullets > 0;

                public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                    new (TargetType.Single) { Side = SideSelector.Opposite, }
                };

                public new Anna User => base.User as Anna;

                public LegShot (Anna user) : base (user) {}

                public BasicAttackOptions AttackOptions = new () {
                    HitRollTags = new [] { "Attack", "Shot" },
                    ParryNegation = 10,
                    DodgeNegation = 1,
                };

                public override async Task Run () {
                    var target = Targets[0];
                    User.Play(User.Animations.Shoot);
                    User.Play(User.Sounds.Shot);
                    User.Bullets -= 1;

                    var result = await User.Attack(target, AttackOptions);

                    if (result.Hit) {
                        var damage = User.Roll(4, "Damage", "Shot");
                        target.Combatant.Damage(damage, new [] { "Bullet" });
                        target.Combatant.AddStatusEffect(new Immobilized (2));
                    }
                }
            }
        
            public class Unload : CombatAction {
                public override string Name => "Unload";
                public override int TempoCost { get; set; } = 3;

                public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                    new (TargetType.Single) { Side = SideSelector.Opposite, },
                };

                public new Anna User => base.User as Anna;
                public Unload (Anna user) : base (user) {}

                public override async Task Run () {
                    var target = Targets[0];

                    User.Play(User.Animations.Shoot);

                    var attack_options = new BasicAttackOptions () {
                        HitRollTags = new string [] { "Attack", "Shot" },
                        ParryNegation = 10,
                        DodgeNegation = 4,
                    };

                    var hit_modifier = User.AddRollModifier(new (this, "Attack") { Advantage = -1, Bonus = 1 });
                    var damage_modifier = User.AddRollModifier(new (this, "Damage") { Advantage = - 1 });

                    for (var i = 0; i < User.Bullets; i++) {
                        var result = await User.Attack(target, attack_options);

                        User.Play(User.Sounds.Shot);

                        if (result.Hit) {
                            var damage = User.Roll(6, "Damage", "Shot");
                            result.Defender.Damage(damage, new string [] { "Shot" });
                        }

                        hit_modifier.Bonus--;

                        await Timing.Delay( 1f / User.Bullets * 2f);
                    }

                    User.Bullets = 0;
                    User.RemoveRollModifier(hit_modifier);
                    User.RemoveRollModifier(damage_modifier);
                }
            }
        }
    }
}