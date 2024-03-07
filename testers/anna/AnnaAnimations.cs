namespace Combat {
    public partial class Anna {
        private static string texture_path = "res://testers/anna/textures";

        public class AnimationStore : StandardAnimationStore {
            public SimpleSprite Shoot { get; set; }
            public SimpleSprite Reload { get; set; }
            public SimpleSprite Kick { get; set; }
            public SimpleSprite Smoke { get; set; }
        }

        protected override StandardAnimationStore StandardAnimations => Animations;

        public AnimationStore Animations = new () {
            Idle = new SimpleAnimation () {
                Sprites = new SimpleSprite [] {
                    new (texture_path, "idle"),
                },
            },
            Hurt = new (texture_path, "hurt"),
            Parry = new (texture_path, "parry"),
            Dodge = new (texture_path, "dodge"),
            Dead = new (texture_path, "dead"),
            Shoot = new (texture_path, "shoot"),
            Reload = new (texture_path, "reload"),
            Kick = new (texture_path, "kick"),
            Smoke = new (texture_path, "smoke"),
        };
    }
}