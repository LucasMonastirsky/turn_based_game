using System;
using System.Threading.Tasks;
using Development;
using Godot;

namespace Combat {
    public partial class ActionButton : CenterContainer {
        [Export] private ActionDisplayButton Button;
        [Export] public string ActionName { get => Action.Name; set { } }

        public bool Disabled {
            get => Button.Disabled;
            set {
                Button.Disabled = value;
                Button.TextureNormal = value ? null : Action?.IconTexture;
            } 
        }
        public bool IsHovered => Button.IsHovered();

        private CombatAction _action;
        public CombatAction Action {
            get => _action;
            set {
                _action = value;

                if (value is null) {
                    Button.Disabled = true;
                    Button.TextureNormal = Button.DefaultTexture;
                }
                else {
                    Button.Disabled = !value.IsAvailable;
                    Button.TextureNormal = _action.IconTexture ?? Button.DefaultTexture;
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
                Dev.Log($"Pressed button while disabled == {Disabled}");
                if (!Disabled) Action.RequestBind();
            };
        }
    }
}