namespace Combat {
    public partial class Anna {
        protected override string resources_path => "res://combatants/characters/anna/resources";

        public class AnimationStore : StandardAnimationStore {
            public SimpleSprite Shoot { get; set; }
            public SimpleSprite Reload { get; set; }
            public SimpleSprite Kick { get; set; }
            public SimpleSprite Smoke { get; set; }
        }

        public override AnimationStore Animations => _animations;
        private AnimationStore _animations;

        protected override void LoadSprites () {
            _animations = new () {
                Idle = new SimpleAnimation () {
                    Sprites = new SimpleSprite [] {
                        LoadSprite("idle"),
                    },
                },
                Hurt = LoadSprite("hurt"),
                Parry = LoadSprite("parry"),
                Dodge = LoadSprite("dodge"),
                Dead = LoadSprite("dead"),
                Shoot = LoadSprite("shoot"),
                Reload = LoadSprite("reload"),
                Kick = LoadSprite("kick"),
                Smoke = LoadSprite("smoke"),
            };
        }
    }
}