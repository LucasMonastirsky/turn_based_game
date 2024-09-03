using System;
using System.Threading.Tasks;
using Godot;

namespace Combat {
    public partial class ActionButton : CenterContainer {
        [Export] private TextureButton Button;
        [Export] private Texture2D DefaultTexture;

        public bool Disabled { get => Button.Disabled; set { Button.Disabled = value; } }

        public Action<CombatAction> OnPressed;

        public TaskCompletionSource<CombatAction> CompletionSource;

        private CombatAction _action;
        public CombatAction Action {
            get => _action;
            set {
                _action = value;

                if (value is null) {
                    Button.Disabled = true;
                    Button.TextureNormal = DefaultTexture;
                }
                else {
                    Button.Disabled = !value.IsAvailable;
                    Button.TextureNormal = _action.IconTexture ?? DefaultTexture;
                }
            }
        }

        public Texture2D Texture {
            get => Button.TextureNormal;
            set {
                Button.TextureNormal = value;
            }
        }

        public override void _EnterTree () {
            Button.Pressed += () => {
                Action.RequestBind();
            };
        }
    }
}