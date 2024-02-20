using System.Collections.Generic;
using System.Threading.Tasks;
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

        public static async Task<CombatAction> RequestAction (Combatant combatant) {
            foreach (var button in current.buttons) {
                button.QueueFree();
            }

            var completion_source = new TaskCompletionSource<CombatAction>();

            current.buttons = new ();

            foreach (var action in combatant.ActionList) {
                var button = current.action_button_scene.Instantiate<CombatPlayerInterfaceActionButton>();
                current.action_button_container.AddChild(button);
                button.Action = action;
                button.CompletionSource = completion_source;
                button.Disabled = !action.IsAvailable() || action.TempoCost > combatant.Tempo;
                current.buttons.Add(button);
            }

            ActionListVisible = true;

            return await completion_source.Task;
        }

        public static void ShowActionList () {
            ActionListVisible = true;
        }

        public static void ShowActionList (Combatant combatant) {
            foreach (var button in current.buttons) {
                button.QueueFree();
            }

            current.buttons = new ();

            foreach (var action in combatant.ActionList) {
                var button = current.action_button_scene.Instantiate<CombatPlayerInterfaceActionButton>();
                current.action_button_container.AddChild(button);
                button.Action = action;
                button.Disabled = !action.IsAvailable() || action.TempoCost > combatant.Tempo;
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