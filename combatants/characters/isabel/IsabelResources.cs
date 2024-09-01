namespace Combat {
    public partial class Isabel {
        protected override string resources_path => "res://combatants/characters/isabel/resources";

        public class AnimationStore : StandardAnimationStore {
            public SimpleSprite Swing { get; set; }
            public SimpleSprite Teleport { get; set; }
            public SimpleSprite Jutsu { get; set; }
        }

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
                Parry = LoadSprite("parry"),
                Dodge = LoadSprite("dodge"),
                Dead = LoadSprite("dead"),
                Swing = LoadSprite("swing"),
                Teleport = LoadSprite("teleport"),
                Jutsu = LoadSprite("jutsu"),
            };
        }
    }
}