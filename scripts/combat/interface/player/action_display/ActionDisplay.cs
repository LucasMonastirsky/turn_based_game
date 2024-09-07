using System.Collections.Generic;
using System.Linq;
using Development;
using Godot;

namespace Combat {
    public partial class ActionDisplay : HBoxContainer {
        private static ActionDisplay Current;

        [Export] private Container ActionButtonContainer;
        [Export] private ActionDetail ActionDetail;

        private List<ActionButton> Buttons;

        public bool Disabled {
            get => !Visible;
            set { Visible = !value; }
        }

        public override void _EnterTree () {
            Current = this;

            Buttons = ActionButtonContainer.GetChildren().Cast<ActionButton>().ToList();
        }

        public static void RequestAction (Combatant combatant) {
            ShowActionList();

            if (combatant.Controller is not PlayerController) {
                Dev.Error($"Requesting actions from non-player combatant");
            }

            CombatantDetail.Combatant = combatant;

            var actions = combatant.ActionList; // TODO: handle display index

			for (var i = 0; i < Current.Buttons.Count; i++) {
				var button = Current.Buttons[i];

                if (i < actions.Count) button.Action = actions[i];
                else button.Action = null;
			}
        }

        public static void ShowActionList () {
            Current.Buttons.ForEach(button => button.Disabled = false);
        }

        public static void HideActionList () {
            Current.Buttons.ForEach(button => button.Disabled = true);
        }

        public static void SetHoveredAction (CombatAction action) {
            Current.ActionDetail.Action = action;
        }

        public override void _Process (double delta) {
            Buttons.ForEach(button => {
                if (button.IsHovered && TurnManager.State == TurnManager.TurnState.Requesting) {
                    SetHoveredAction(button.Action);
                }
            });
        }
    }
}