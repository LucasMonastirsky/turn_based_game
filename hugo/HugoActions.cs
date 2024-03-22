using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Combat;
using static Dice;

public partial class Hugo {
    public override List<CombatAction> ActionList => FetchActionsFrom(Actions);

    public ActionStore Actions;

    public class ActionStore {
        public HugoActions.Swing Swing;
        public HugoActions.Shove Shove;
        public HugoActions.Blast Blast;

        public CommonActions.Move Move;
        public CommonActions.Switch Switch;
        public CommonActions.Pass Pass;

        public ActionStore (Hugo hugo) {
            foreach (var field in GetType().GetFields()) {
                field.SetValue(this, Activator.CreateInstance(field.FieldType, hugo));
            }
        }
    }

    public abstract class HugoActions {
        public class Swing : CombatAction {
            public override string Name { get => "Swing"; }
            public override int TempoCost { get; set; } = 2;

            public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                CommonTargetSelectors.Melee,
            };

            public override List<ActionRestrictor> Restrictors { get; init; } = new () {
                ActionRestrictors.FrontRow,
            };

            public new Hugo User { get => base.User as Hugo; }

            public Swing (Combatant user) : base(user) {}

            public override async Task Run () {
                var target = Targets[0];

                await User.Attack(target, new () {
                    RollTags = new [] { "Melee", "Armed", },
                    ParryNegation = 3,
                    DodgeNegation = 2,
                    DamageRoll = D8.Plus(2),
                    Sprite = User.Animations.Swing,
                    MoveToMeleeDistance = true,
                });
            }
        }

        public class Blast : CombatAction {
            public override string Name => "Blast";
            public override int TempoCost { get; set; } = 2;

            public new Hugo User => base.User as Hugo;

            public Blast (Combatant user) : base (user) {}

            public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                new (TargetType.Single) { Side = SideSelector.Opposite, }
            };

            public override List<ActionRestrictor> Restrictors { get; init; } = new () {
                ActionRestrictors.BackRow,
            };

            public override async Task Run() {
                var target = Targets[0];

                await User.Attack(target, new () {
                    RollTags = new string [] { "Ranged", },
                    ParryNegation = 8,
                    DodgeNegation = 4,
                    DamageRoll = D4.Plus(2),
                    Sprite = User.Animations.Blast,
                });
            }
        }

        public class Shove : CombatAction {
            public override string Name => "Shove";
            public override int TempoCost { get; set; } = 2;

            public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                CommonTargetSelectors.Melee,
                new (TargetType.Position) {
                    Side = SideSelector.Opposite,
                    Row = 1,
                    Validator = (target, user, targets) => (
                        target.Combatant is null
                        && target.VerticalDistanceTo(targets[0]) <= 1
                        && Positioner.IsValidMovement(targets[0].Combatant, target.Position, true)
                    ),
                },
            };

            public override List<ActionRestrictor> Restrictors { get; init; } = new () {
                ActionRestrictors.FrontRow,
            };

            public new Hugo User => base.User as Hugo;

            public Shove (Combatant user) : base (user) {}

            public override async Task Run () {
                var attack = new AttackOptions () {
                    RollTags = new [] { "Melee", "Unarmed", },
                    ParryNegation = 2,
                    DodgeNegation = 4,
                    DamageRoll = D4,
                    Sprite = User.Animations.Shove,
                    MoveToMeleeDistance = true,
                };

                await User.Attack(Targets[0], attack, async result => {
                    if (!result.Dodged) {
                        await Targets[0].Combatant.MoveTo(Targets[1].Position);
                    }
                });
            }
        }
    }
}