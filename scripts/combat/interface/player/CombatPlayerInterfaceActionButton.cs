using System.Threading.Tasks;
using Godot;

namespace Combat {
    public partial class CombatPlayerInterfaceActionButton : Button {
        public TaskCompletionSource<CombatAction> CompletionSource;

        private CombatAction _action;
        public CombatAction Action {
            get => _action;
            set {
                _action = value;
                Text = $"{_action.Name} ({value.TempoCost})";
                Pressed += async () => { // TODO: try/catch here to handle cancelling?
                    var bound_action = await _action.RequestBind();
                    if (bound_action is null) CombatPlayerInterface.ShowActionList();
                    else CompletionSource.SetResult(bound_action);
                };
            }
        }
    }
}