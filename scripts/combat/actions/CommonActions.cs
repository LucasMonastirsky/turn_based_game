using System;
using System.Threading.Tasks;

namespace Combat {
    public static class CommonActions {
        public class Pass : CombatAction {
            public override string Name => "Pass";

            public Pass (ICombatant user) : base (user) {}

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