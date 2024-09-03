using System;
using System.Collections.Generic;
using System.Linq;
using Development;
using Godot;


namespace Combat {
    public partial class ActionDisplay : HBoxContainer {

        public bool Disabled {
            get => !Visible;
            set { Visible = !value; }
        }

        private static ActionDisplay Current;

        [Export] private Container ActionButtonContainer;
        [Export] private PackedScene ActionButtonScene;

        private List<ActionButton> Buttons;

        public override void _EnterTree () {
            Current = this;

            Buttons = ActionButtonContainer.GetChildren().Cast<ActionButton>().ToList();
        }

        public static void RequestAction (Combatant combatant) {
            ShowActionList();

            if (combatant.Controller is not PlayerController) {
                Dev.Error($"Requesting actions from non-player combatant");
            }

            var actions = combatant.ActionList; // TODO: handle display index

			for (var i = 0; i < Current.Buttons.Count; i++) {
				var button = Current.Buttons[i];

                if (i < actions.Count) button.Action = actions[i];
                else button.Action = null;
			}
        }

        public static void ShowActionList () {
            Current.Visible = true;
        }

        public static void HideActionList () {
            Current.Visible = false;
        }
    }
}