namespace Combat {
    public partial class Ghoul {
        public override string resources_path => "res://combatants/enemies/ghoul/resources";

        public class AnimationStore : StandardAnimationStore {
            public SimpleSprite Punch { get; set; }
            public SimpleSprite Charge { get; set; }
        }

        public override AnimationStore Animations => _animations;
        private AnimationStore _animations;

        protected override void LoadSprites () {
            _animations = new ();
            LoadStandardSprites();
            _animations.Punch = LoadSprite("punch");
            _animations.Charge = LoadSprite("charge");
        }
    }
}