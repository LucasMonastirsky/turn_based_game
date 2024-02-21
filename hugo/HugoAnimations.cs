using Combat;

public partial class Hugo : Combatant {
    private static string texture_path = "res://hugo/textures";
    protected class AnimationStore : StandardAnimationStore {
        public override SimpleAnimation Idle { get; set; }
        public override SimpleSprite Hurt { get; set; }
        public override SimpleSprite Parry { get; set; }
        public override SimpleSprite Dodge { get; set; }
        public override SimpleSprite Dead { get; set; }
        public SimpleSprite Swing { get; set; }
        public SimpleSprite Blast { get; set; }
        public SimpleSprite Shove { get; set; }
    }
    protected override StandardAnimationStore StandardAnimations { get => Animations; }
    protected AnimationStore Animations = new () {
        Idle = new SimpleAnimation() {
            Sprites = new SimpleSprite[] {
                new SimpleSprite(texture_path, "idle_0"),
                new SimpleSprite(texture_path, "idle_1"),
            },
        },
        Hurt = new SimpleSprite(texture_path, "hurt"),
        Parry = new SimpleSprite(texture_path, "parry"),
        Dodge = new SimpleSprite(texture_path, "dodge"),
        Dead = new SimpleSprite(texture_path, "dead"),
        Swing = new SimpleSprite(texture_path, "swing"),
        Blast = new SimpleSprite(texture_path, "blast"),
        Shove = new SimpleSprite(texture_path, "shove"),
    };
}