using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Combat;

public partial class Miguel {
    public override List<CombatAction> ActionList => FetchActionsFrom(Actions);

    public ActionStore Actions;

    public class ActionStore {
        public ActionClasses.Swing Swing;
        public ActionClasses.Switcheroo Switcheroo;
        public ActionClasses.Combo Combo;
        public ActionClasses.Immobilize Immobilize;

        public CommonActions.Move Move;
        public CommonActions.Switch Switch;
        public CommonActions.Pass Pass;

        public ActionStore (Miguel miguel) {
            foreach (var field in typeof(ActionStore).GetFields()) {
                field.SetValue(this, Activator.CreateInstance(field.FieldType, miguel));
            }
        }
    }

    public class ActionClasses {
        public class Swing : CombatAction {
            public override string Name => "Swing";
            public override int TempoCost { get; set; } = 2;

            public override bool IsAvailable () {
                return User.Row == 0;
            }

            public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                CommonTargetSelectors.Melee,
            };

            public new Miguel User => base.User as Miguel;
            public Swing (Miguel user) : base (user) {}

            public override async Task Run () {
                var target = Targets[0];

                User.Animator.Play(User.Animations.Swing);
                await User.DisplaceToMeleeDistance(target.Combatant);

                var options = new BasicAttackOptions () {
                    HitRollTags = new [] { "Attack", "Melee", },
                    ParryNegation = 2, DodgeNegation = 0,
                };

                await User.Attack(target, options, async result => {
                    if (result.Hit) {
                        var damage_roll = User.Roll(8, new string [] { "Damage" });
                        target.Combatant.Damage(damage_roll + 4, new string [] { "Cut" });
                        target.Combatant.AddStatusEffect(new Poison(3));
                    }
                });
            }
        }

        public class Switcheroo : CombatAction {
            public override string Name => "Switcheroo";
            public override int TempoCost { get; set; } = 1;

            public override bool IsAvailable () => User.Row == 1;

            public new Miguel User => base.User as Miguel;

            public Switcheroo (Combatant user) : base(user) {}

            public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                new TargetSelector(TargetType.Single) {
                    Side = SideSelector.Same,
                    Row = 0,
                    Validator = (target, user, previous_targets) => !target.Combatant.HasStatusEffect<SwitcherooEffect>()
                },
            };

            public override async Task Run () {
                var target = Targets[0];

                User.Animator.Play(User.Animations.Seal);
                target.Combatant.AddStatusEffect(new SwitcherooEffect (User));
            }

            public class SwitcherooEffect : StatusEffect {
                public override string Name => "Switcheroo";

                public Combatant Caster;

                public SwitcherooEffect (Combatant caster) {
                    Caster = caster;
                }

                public override void OnApplied () {
                    CombatEvents.AfterDeath.Until(async arguments => {
                        if (arguments.Combatant == Caster) {
                            User.RemoveStatusEffect(this);
                            return true;
                        }

                        return false;
                    });

                    CombatEvents.BeforeAttack.Until(async arguments => {
                        if (Caster.IsDead || !User.HasStatusEffect(this)) return true;

                        if (arguments.Target.Combatant != User || TurnManager.ActiveCombatant == User) {
                            return false;
                        }
                        else {
                            await Positioner.SwitchCombatants(User, Caster);
                            Caster.AddRollModifier(new (this, "Parry") { Advantage = 1, Temporary = true, });
                            Caster.AddRollModifier(new (this, "Attack") { Advantage = 1, Temporary = true, });
                            
                            foreach (var combatant in Battle.Combatants) {
                                combatant.RemoveStatusEffectIf<SwitcherooEffect>(effect => effect.Caster == Caster);
                            }

                            return true;
                        }
                    });
                }
            }
        }

        public class Combo : CombatAction {
            public override string Name => "Combo";
            public override int TempoCost { get; set; } = 3;

            public new Miguel User => base.User as Miguel;

            public Combo (Miguel user) : base (user) {}

            public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                CommonTargetSelectors.Melee,
            };

            public BasicAttackOptions [] AttackOptions { get; protected set; } = new [] {
                new BasicAttackOptions () {
                    HitRollTags = new [] { "Attack", "Melee", "Weapon", },
                    ParryNegation = 4,
                    DodgeNegation = 2,
                },
                new BasicAttackOptions () {
                    HitRollTags = new [] { "Attack", "Melee", "Unarmed", },
                    ParryNegation = 2,
                    DodgeNegation = 6,
                },
                new BasicAttackOptions () {
                    HitRollTags = new [] { "Attack", "Melee", "Unarmed", },
                    ParryNegation = 2,
                    DodgeNegation = 6,
                },
            };

            public override async Task Run() {
                var target = Targets[0];

                await User.DisplaceToMeleeDistance(target.Combatant);

                var first_attack = await User.Attack(target, AttackOptions[0]);
                User.Animator.Play(User.Animations.Swing);
                if (first_attack.Hit) {
                    var damage_roll = User.Roll(8, new string [] { "Damage" });
                    target.Combatant.Damage(damage_roll + 4, new string [] { "Cut" });
                }

                if (!first_attack.Parried) {
                    await Timing.Delay();

                    var second_attack = await User.Attack(target, AttackOptions[1]);
                    if (second_attack.Hit) {
                        var damage_roll = User.Roll(8, new string [] { "Damage" });
                        target.Combatant.Damage(damage_roll, new string [] { "Blunt" });
                    }

                    User.Animator.Play(User.Animations.Combo_1);

                    await Timing.Delay();

                    var third_attack = await User.Attack(target, AttackOptions[2]);
                    if (third_attack.Hit) {
                        var damage_roll = User.Roll(8, new string [] { "Damage" });
                        target.Combatant.Damage(damage_roll, new string[] { "Blunt" });
                    }

                    User.Animator.Play(User.Animations.Combo_2);
                }
            }
        }


        public class Immobilize : CombatAction {
            public override string Name => "Immobilize";
            public override int TempoCost { get; set; } = 1;

            public new Miguel User => base.User as Miguel;

            public Immobilize (Miguel user) : base (user) {}

            public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                new (TargetType.Single) { Side = SideSelector.Opposite }
            };

            public override async Task Run () {
                User.Animator.Play(User.Animations.Seal);
                Targets[0].Combatant.AddStatusEffect(new Immobilized (1));
            }

        }
    }
}