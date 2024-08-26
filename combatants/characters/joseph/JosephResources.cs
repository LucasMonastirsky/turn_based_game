namespace Combat {
    public partial class Joseph {
        private static string texture_path = "res://combatants/characters/joseph/textures";

        public class AnimationStore : StandardAnimationStore {
            public SimpleSprite Swing { get; set; }
            public SimpleSprite BigSwing { get; set; }
            public SimpleSprite Stab { get; set; }
            public SimpleSprite Point { get; set; }
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
            BigSwing = new (texture_path, "big_swing"),
            Stab = new (texture_path, "stab"),
            Point = new (texture_path, "point"),
        };
    }
}