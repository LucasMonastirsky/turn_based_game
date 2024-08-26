using System.Collections.Generic;
using System.Threading.Tasks;

namespace Combat {
    public abstract class MeleeAction : CombatAction {
        public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
            CommonTargetSelectors.Melee,
        };

        public override List<ActionRestrictor> Restrictors { get; init; } = new () {
            ActionRestrictors.FrontRow,
        };

        public MeleeAction (Combatant user) : base (user) {}
    }

    public static class CommonActions {
        public class Move : CombatAction {
            public override string Name { get => "Move"; }
            public override int TempoCost { get; set; } = 1;

            public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                new TargetSelector (TargetType.Position) {
                    Side = SideSelector.Same,
                    Validator = (target, user, previous_targets) => {
                        return user.CanMoveTo(target.Position);
                    }
                },
            };

            public override List<ActionRestrictor> Restrictors { get; init; } = new () {
                ActionRestrictors.CanMove,
            };

            public Move (Combatant user) : base (user) {}

            public override async Task Run () {
                await User.MoveTo(Target);
            }
        }

        public class Pass : CombatAction {
            public override string Name => "Pass";
            public override int TempoCost { get; set; } = 0;

            public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {};

            public Pass (Combatant user) : base (user) {}

            public override async Task Run () {
                TurnManager.PassTurn();
            }
        }
    }
}