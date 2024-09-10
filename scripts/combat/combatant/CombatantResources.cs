using Godot;
using ResourceHelpers;

namespace Combat {
    public partial class Combatant {
        public abstract string resources_path { get; }

        public Texture2D Icon = null;

        protected SimpleSprite LoadSprite (string name, Vector2? offset = null) {
            var sprite = new SimpleSprite (resources_path, $"sprites/{name}");

            if (offset != null) sprite.Offset = (Vector2) offset;

            return sprite;
        }
        protected virtual void LoadSprites () {} // TODO: abstract this

        public void LoadResources () {
            LoadSprites();
            Icon = Resources.LoadTexture(resources_path, "icons/icon_profile");
        }

        protected void LoadStandardSprites () {
            Animations.Idle = new SimpleAnimation () {
                Sprites = new SimpleSprite [] {
                    LoadSprite("idle_0"),
                    LoadSprite("idle_1"),
                },
            };
            Animations.Hurt = LoadSprite("hurt");
            Animations.Parry = LoadSprite("parry");
            Animations.Dodge = LoadSprite("dodge");
        }
    }
}