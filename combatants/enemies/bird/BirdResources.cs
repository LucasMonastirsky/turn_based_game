namespace Combat {
    public partial class Bird {
        private static string texture_path = "res://combatants/enemies/bird/textures";

        public class AnimationStore : StandardAnimationStore {
            public SimpleSprite Peck { get; set; }
            public SimpleSprite Screech { get; set; }
        }

        public override StandardAnimationStore StandardAnimations => Animations;

        public AnimationStore Animations = new () {
            Idle = new SimpleAnimation () {
                Sprites = new SimpleSprite [] {
                    new (texture_path, "idle_0"),
                    new (texture_path, "idle_1"),
                },
            },
            Hurt = new (texture_path, "hurt"),
            Parry = new (texture_path, "parry"),
            Dodge = new (texture_path, "dodge"),
            Dead = new (texture_path, "dead"),
            Peck = new (texture_path, "peck"),
            Screech = new (texture_path, "screech"),
        };
    }
}