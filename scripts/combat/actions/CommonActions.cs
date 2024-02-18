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

            public Move (Combatant user) : base (user) {}

            public override async Task Run () {
                var target = Targets[0];
                if (target.Combatant != null) target.Combatant.Tempo--;
                await Positioner.SwitchPosition(User, target.Position);
            }
        }

        public class Pass : CombatAction {
            public override string Name => "Pass";
            public override int TempoCost { get; set; } = 0;

            public override List<TargetSelector> TargetSelectors { get; protected set; } = new () {

            };

            public Pass (Combatant user) : base (user) {}

            public override async Task Run () {
                await TurnManager.EndTurn();
            }
        }
    }
}