namespace Combat {
    public partial class Combatant {
        protected abstract string resources_path { get; }

        protected SimpleSprite LoadSprite (string name) => new SimpleSprite (resources_path, $"sprites/{name}");
        protected virtual void LoadSprites () {} // TODO: abstract this

        public void LoadResources () {
            LoadSprites();
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