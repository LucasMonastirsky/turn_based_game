namespace Combat {
    public partial class Bird {
        public override string resources_path => "res://combatants/enemies/bird/resources";

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
                        LoadSprite("idle_0", new (0, -10)),
                        LoadSprite("idle_1", new (0, -10)),
                    },
                },
                Hurt = LoadSprite("hurt", new (0, -10)),
                Parry = LoadSprite("parry", new (0, -10)),
                Dodge = LoadSprite("dodge", new (0, -10)),
                Peck = LoadSprite("peck", new (0, -10)),
                Screech = LoadSprite("screech", new (0, -10)),
            };
        }
    }
}