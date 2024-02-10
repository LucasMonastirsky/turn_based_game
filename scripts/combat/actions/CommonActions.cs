using System.Threading.Tasks;

namespace Combat {
    public static class CommonActions {
        public class Move : CombatAction {
            public override string Name { get => "Move"; }

            public Move (Combatant user) : base (user) {}

            private CombatPosition bound_target_position;
            public Move Bind (CombatPosition bound_target_position) {
                this.bound_target_position = bound_target_position;
                Bound = true;
                return this;
            }

            public override async Task Run () {
                await Positioner.SwitchPosition(User, bound_target_position);
            }

            public override async Task RequestTargetsAndRun () {
                CombatPlayerInterface.HideActionList();

                var target_position = await User.Controller.RequestPosition(User);

                if (target_position == null) {
                    CombatPlayerInterface.ShowActionList();
                    return;
                }

                await InteractionManager.Act(Bind((CombatPosition) target_position));
            }
        }

        public class Pass : CombatAction {
            public override string Name => "Pass";

            public Pass (Combatant user) : base (user) {}

            public Pass Bind () {
                Bound = true;
                return this;
            }

            public override async Task Run () {

            }

            public override async Task RequestTargetsAndRun () {
                CombatPlayerInterface.HideActionList();
                await InteractionManager.Act(Bind());
            }
        }
    }
}