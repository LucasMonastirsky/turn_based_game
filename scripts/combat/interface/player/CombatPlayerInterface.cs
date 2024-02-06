using System.Collections.Generic;
using Godot;

namespace Combat {
    public partial class CombatPlayerInterface : Control {
        private static CombatPlayerInterface current;

        private static bool _action_list_visible;
        public static bool ActionListVisible {
            get => _action_list_visible;
            set {
                current.Visible = value;
                _action_list_visible = value;
            }
        }

        [Export] private PackedScene action_button_scene;
        
        [Export] private GridContainer action_button_container;

        private List<CombatPlayerInterfaceActionButton> buttons = new ();


        public override void _EnterTree () {
            current = this;
        }

        public static void ShowActionList () {
            ActionListVisible = true;
        }

        public static void ShowActionList (List<CombatAction> actions) {
            foreach (var button in current.buttons) {
                button.QueueFree();
            }

            current.buttons = new ();

            foreach (var action in actions) {
                var button = current.action_button_scene.Instantiate<CombatPlayerInterfaceActionButton>();
                current.action_button_container.AddChild(button);
                button.Action = action;
                button.Disabled = !action.IsAvailable();
                current.buttons.Add(button);
            }

            ActionListVisible = true;
        }

        public static void HideActionList () {
            ActionListVisible = false;
        }

        public static void ClearActionList () {
            foreach (var button in current.buttons) {
                button.QueueFree();
            }

            current.buttons = new ();
        }
    }
}