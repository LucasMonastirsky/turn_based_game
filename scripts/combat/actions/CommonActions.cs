using System;
using System.Threading.Tasks;

namespace Combat {
    public static class CommonActions {
        public class Move : CombatAction {
            public override string Name { get => "Move"; }

            public Move (Combatant user) : base (user) {}

            public async Task Run (CombatPosition target_position) {
                Positioner.SwitchPosition(user, target_position);

                await InteractionManager.ResolveQueue();
                await InteractionManager.EndAction();
            }

            public override async Task RequestTargetsAndRun () {
                var target_position = await user.Controller.RequestPosition(user);
                Run(target_position);
            }
        }

        public class Pass : CombatAction {
            public override string Name => "Pass";

            public Pass (Combatant user) : base (user) {}

            public async Task Run () {
                await InteractionManager.ResolveQueue();
                await InteractionManager.EndAction();
            }

            public override async Task RequestTargetsAndRun () {
                Run();
            }
        }
    }
}