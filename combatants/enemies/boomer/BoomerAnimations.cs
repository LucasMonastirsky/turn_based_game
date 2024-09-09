namespace Combat {
    public partial class Boomer {
        public override string resources_path => "res://combatants/enemies/boomer/resources";

        public override AnimationStore Animations => _animations;
        private AnimationStore _animations;

        protected override void LoadSprites () {
            _animations = new () {
                Idle = new SimpleAnimation () {
                    Sprites = new SimpleSprite [] {
                        LoadSprite("idle_0"),
                        LoadSprite("idle_1"),
                    }
                },
                Hurt = LoadSprite("hurt"),
                Spew = LoadSprite("spew"),
                Charge = LoadSprite("charge"),
                Explode = LoadSprite("explode"),
            };
        }

        public class AnimationStore : StandardAnimationStore {
            public SimpleSprite Spew { get; set; }
            public SimpleSprite Charge { get; set; }
            public SimpleSprite Explode { get; set; }
        }
    }
}