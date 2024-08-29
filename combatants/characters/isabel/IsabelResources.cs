namespace Combat {
    public partial class Isabel {
        private static string texture_path = "res://combatants/characters/isabel/textures";

        public class AnimationStore : StandardAnimationStore {
            public SimpleSprite Swing { get; set; }
            public SimpleSprite Teleport { get; set; }
            public SimpleSprite Jutsu { get; set; }
        }

        protected override StandardAnimationStore StandardAnimations => Animations;

        public AnimationStore Animations = new () {
            Idle = new SimpleAnimation () {
                Sprites = new SimpleSprite [] {
                    new (texture_path, "idle_0"),
                    new (texture_path, "idle_1"),
                }
            },
            Hurt = new (texture_path, "hurt"),
            Parry = new (texture_path, "parry"),
            Dodge = new (texture_path, "dodge"),
            Dead = new (texture_path, "dead"),
            Swing = new (texture_path, "swing"),
            Teleport = new (texture_path, "teleport"),
            Jutsu = new (texture_path, "jutsu"),
        };
    }
}