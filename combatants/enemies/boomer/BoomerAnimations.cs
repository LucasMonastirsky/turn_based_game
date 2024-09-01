namespace Combat {
    public partial class Boomer {
        private static string texture_path = "res://combatants/enemies/boomer/textures";

        public override StandardAnimationStore StandardAnimations => Animations;

        public AnimationStore Animations = new () {
            Idle = new SimpleAnimation () {
                Sprites = new SimpleSprite [] {
                    new SimpleSprite (texture_path, "idle_0"),
                    new SimpleSprite (texture_path, "idle_1"),
                }
            },
            Dead = new SimpleSprite (texture_path, "dead"),
            Hurt = new SimpleSprite (texture_path, "hurt"),
            Spew = new SimpleSprite (texture_path, "spew"),
            Charge = new SimpleSprite (texture_path, "charge"),
            Explode = new SimpleSprite (texture_path, "explode"),
        };

        public class AnimationStore : StandardAnimationStore {
            public SimpleSprite Spew { get; set; }
            public SimpleSprite Charge { get; set; }
            public SimpleSprite Explode { get; set; }
        }
    }
}