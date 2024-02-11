using System.Collections.Generic;
using System.Threading.Tasks;

namespace Combat {
    public static class CommonActions {
        public class Move : CombatAction {
            public override string Name { get => "Move"; }
            public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {
                new TargetSelector (TargetType.Position) {
                    Side = SideSelector.Same,
                    Validator = (target, user, previous_targets) => user.CanMoveTo(target.Position),
                },
            };

            public Move (Combatant user) : base (user) {}

            public override async Task Run () {
                await Positioner.SwitchPosition(User, Targets[0].Position);
            }
        }

        public class Pass : CombatAction {
            public override string Name => "Pass";

            public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {

            };

            public Pass (Combatant user) : base (user) {}

            public override async Task Run () {

            }
        }
    }
}