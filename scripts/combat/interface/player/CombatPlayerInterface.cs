using System.Collections.Generic;
using System.ComponentModel.Design;
using CustomDebug;
using Godot;

namespace Combat {
    public partial class CombatPlayerInterface : Control {
        private static CombatPlayerInterface current;

        [Export] private PackedScene action_button_scene;
        
        [Export] private GridContainer action_button_container;

        private List<CombatPlayerInterfaceActionButton> buttons = new ();

        private Vector2 debug_marker_position;

        public override void _EnterTree () {
            current = this;
        }

        public override void _Draw () {
            DrawCircle(debug_marker_position, 50f, Colors.Aquamarine);
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
                current.buttons.Add(button);
            }
        }

        public static void MoveDebugMarker (Vector2 target) {
            current.debug_marker_position = target;
        }
    }
}