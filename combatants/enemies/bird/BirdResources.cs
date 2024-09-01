namespace Combat {
    public partial class Bird {
        protected override string resources_path => "res://combatants/enemies/bird/resources";

        public class AnimationStore : StandardAnimationStore {
            public SimpleSprite Peck { get; set; }
            public SimpleSprite Screech { get; set; }
        }

        public override AnimationStore Animations => _animations;
        private AnimationStore _animations;

        protected override void LoadSprites () {
            _animations = new () {
                Idle = new () {
                    Sprites = new [] {
                        LoadSprite("idle_0"),
                        LoadSprite("idle_1"),
                    },
                },
                Hurt = LoadSprite("hurt"),
                Parry = LoadSprite("parry"),
                Dodge = LoadSprite("dodge"),
                Dead = LoadSprite("dead"),
                Peck = LoadSprite("peck"),
                Screech = LoadSprite("screech"),
            };
        }
    }
}