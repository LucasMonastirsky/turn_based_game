using System.Threading.Tasks;

namespace Combat {
    public static class CommonActions {
        public class Move : CombatAction {
            public override string Name { get => "Move"; }

            public Move (Combatant user) : base (user) {}

            public async Task Run (CombatPosition target_position) {
                await Positioner.SwitchPosition(user, target_position);
            }

            public override async Task RequestTargetsAndRun () {
                CombatPlayerInterface.HideActionList();

                var target_position = await user.Controller.RequestPosition(user);

                if (target_position == null) {
                    CombatPlayerInterface.ShowActionList();
                    return;
                }

                InteractionManager.RunAction(() => Run((CombatPosition) target_position));
            }
        }

        public class Pass : CombatAction {
            public override string Name => "Pass";

            public Pass (Combatant user) : base (user) {}

            public async Task Run () {

            }

            public override async Task RequestTargetsAndRun () {
                CombatPlayerInterface.HideActionList();
                InteractionManager.RunAction(() => Run());
            }
        }
    }
}