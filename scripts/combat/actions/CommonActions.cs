using System.Collections.Generic;
using System.Threading.Tasks;

namespace Combat {
    public static class CommonActions {
        public class Move : CombatAction {
            public override string Name { get => "Move"; }
            public override int TempoCost { get; set; } = 1;

            public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                new TargetSelector (TargetType.Position) {
                    Side = SideSelector.Same,
                    Validator = (target, user, previous_targets) => {
                        if (target.Combatant != null && target.Combatant.Tempo <= 1) return false;
                        else return user.CanMoveTo(target.Position);
                    }
                },
            };

            public override List<ActionRestrictor> Restrictors { get; init; } = new () {
                ActionRestrictors.CanMove,
            };

            public Move (Combatant user) : base (user) {}

            public override async Task Run () {
                await User.MoveTo(Targets[0]);
            }
        }

        public class Pass : CombatAction {
            public override string Name => "Pass";
            public override int TempoCost { get; set; } = 0;

            public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {

            };

            public Pass (Combatant user) : base (user) {}

            public override async Task Run () {
                TurnManager.PassTurn();
            }
        }
    }
}