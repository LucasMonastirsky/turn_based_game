namespace Combat {
    public partial class Ghoul {
        private static string texture_path = "res://combatants/enemies/ghoul/textures";

        public class AnimationStore : StandardAnimationStore {
            public SimpleSprite Punch { get; set; }
            public SimpleSprite Charge { get; set; }
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
            Punch = new (texture_path, "punch"),
            Charge = new (texture_path, "charge"),
        };
    }
}