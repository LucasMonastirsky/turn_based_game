using System;
using Development;
using Godot;

namespace Combat {
    public partial class CombatPlayerInterfaceActionButton : Button {
        private CombatAction _action;
        public CombatAction Action {
            get => _action;
            set {
                _action = value;
                Text = _action.Name;
                Pressed += async () => { // TODO: try/catch here to handle cancelling?
                    _action.RequestTargetsAndRun();
                };
            }
        }
    }
}