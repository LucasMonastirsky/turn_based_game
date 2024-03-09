using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Combat {
    public partial class Hidan {
        public override List<CombatAction> ActionList => FetchActionsFrom(Actions);

        public ActionStore Actions;

        public class ActionStore {
            public ActionClasses.Stab Stab;
            public ActionClasses.Sweep Sweep;

            public CommonActions.Move Move;
            public CommonActions.Switch Switch;
            public CommonActions.Pass Pass;

            public ActionStore (Hidan hidan) {
                foreach (var field in typeof(ActionStore).GetFields()) {
                    field.SetValue(this, Activator.CreateInstance(field.FieldType, hidan));
                }
            }
        }

        public static class ActionClasses {
            public class Stab : CombatAction {
                public override string Name => "Stab";

                public override int TempoCost { get; set; } = 2;

                public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                    CommonTargetSelectors.Melee,
                };

                public new Hidan User => base.User as Hidan;

                public Stab (Hidan user) : base (user) {}

                public BasicAttackOptions AttackOptions = new () {
                    HitRollTags = new string [] { "Attack", "Melee", "Armed", },
                    ParryNegation = 4,
                    DodgeNegation = 2,
                };

                public override async Task Run () {
                    var target = Targets[0];

                    await User.DisplaceToMeleeDistance(target);
                    var result = await User.Attack(target, AttackOptions);
                    User.Play(User.Animations.Stab);

                    if (result.Hit) {
                        var damage = User.Roll(10, "Damage", "Melee", "Armed") + 4;
                        target.Combatant.Damage(damage, new string [] { "Melee", "Armed" });
                    }
                }
            }
        
            public class Sweep : CombatAction {
                public override string Name => "Sweep";
                public override int TempoCost { get; set; } = 2;

                public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                    new (TargetType.Double) { Side = SideSelector.Opposite, Row = 0, VerticalRange = 1 }
                };
                
                public new Hidan User => base.User as Hidan;
                public Sweep (Hidan user) : base (user) {}

                public BasicAttackOptions AttackOptions = new () {
                    HitRollTags = new string [] { "Attack", "Melee", "Armed" },
                    ParryNegation = 4,
                    DodgeNegation = 1,
                };

                public override async Task Run () {
                    var target = Targets[0];
                    var real_targets = new CombatTarget [] {
                        new CombatTarget (target.Position with { Slot = target.Slot - 1 }),
                        new CombatTarget (target.Position with { Slot = target.Slot + 1 }),
                    };

                    await User.DisplaceToMeleeDistance(target);

                    User.Play(User.Animations.Sweeps[0]);
                    var first_attack = await User.Attack(real_targets[0], AttackOptions);
                    if (first_attack.Hit) {
                        var damage = User.Roll(6, "Damage", "Melee", "Armed");
                        real_targets[0].Combatant.Damage(damage, new string [] { "Melee" });
                    }

                    if (!first_attack.Parried) {
                        await Timing.Delay();
                        User.Play(User.Animations.Sweeps[1]);
                        await User.Attack(real_targets[1], AttackOptions, async result => {
                            if (result.Hit) {
                                var damage = User.Roll(6, "Damage", "Melee", "Armed");
                                real_targets[1].Combatant.Damage(damage, new string [] { "Melee" });
                            }
                        });
                    }
                }
            }
        }
    }
}