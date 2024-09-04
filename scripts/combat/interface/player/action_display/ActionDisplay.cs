using System.Collections.Generic;
using System.Linq;
using Development;
using Godot;

namespace Combat {
    public partial class ActionDisplay : HBoxContainer {
        private static ActionDisplay Current;

        [Export] private Container ActionButtonContainer;
        [Export] private RichTextLabel ActionDetailTitle, ActionDetailDescription;
        [Export] private TextureRect ProfileIcon;

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

            Current.ProfileIcon.Texture = combatant.Icon;

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

        public static void SetHoveredAction (CombatAction action) {
            Current.ActionDetailTitle.Text = action.Name;
        }

        public override void _Process (double delta) {
            ActionDetailTitle.Text = "";

            Buttons.ForEach(button => {
                if (button.IsHovered && button.Action is not null) {
                    ActionDetailTitle.Text = button.Action.Name;
                }
            });
        }
    }
}