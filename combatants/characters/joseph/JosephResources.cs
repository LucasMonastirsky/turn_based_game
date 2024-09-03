namespace Combat {
    public partial class Joseph {
        public override string resources_path => "res://combatants/characters/joseph/resources";

        public override AnimationStore Animations => _animations;
        private AnimationStore _animations;

        public class AnimationStore : StandardAnimationStore {
            public SimpleSprite Swing { get; set; }
            public SimpleSprite BigSwing { get; set; }
            public SimpleSprite Stab { get; set; }
            public SimpleSprite Point { get; set; }
        }

        protected override void LoadSprites () {
            _animations = new () {
                Idle = new SimpleAnimation () {
                    Sprites = new SimpleSprite [] {
                        LoadSprite("idle_0"),
                        LoadSprite("idle_1"),
                    }
                },
                Hurt = LoadSprite("hurt"),
                Parry = LoadSprite("parry"),
                Dodge = LoadSprite("dodge"),
                Dead = LoadSprite("dead"),
                Swing = LoadSprite("swing"),
                BigSwing = LoadSprite("big_swing"),
                Stab = LoadSprite("stab"),
                Point = LoadSprite("point"),
            };
        }
    }
}