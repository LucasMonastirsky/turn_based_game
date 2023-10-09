using Combat;
using Godot;
using ResourceHelpers;

public partial class Hugo : StandardCombatant {
    private static string texture_path = "res://hugo/textures";
    protected class AnimationStore : StandardAnimationStore {
        public override SimpleAnimation Idle { get; set; }
        public override SimpleSprite Hurt { get; set; }
        public override SimpleSprite Parry { get; set; }
        public override SimpleSprite Dodge { get; set; }
        public SimpleSprite Swing { get; set; }
    }
    protected override StandardAnimationStore StandardAnimations { get => Animations; }
    protected AnimationStore Animations = new () {
        Idle = new SimpleAnimation() {
            Sprites = new SimpleSprite[] {
                new SimpleSprite() {
                    Texture = Resources.LoadTexture(texture_path, "idle_0"),
                    Offset = new Vector2(0, 0),
                },
                new SimpleSprite() {
                    Texture = Resources.LoadTexture(texture_path, "idle_1"),
                    Offset = new Vector2(0, 0),
                },
            },
        },
        Swing = new SimpleSprite() {
            Texture = Resources.LoadTexture(texture_path, "swing"),
        },
        Hurt = new SimpleSprite(texture_path, "hurt"),
        Parry = new SimpleSprite(texture_path, "parry"),
        Dodge = new SimpleSprite(texture_path, "dodge"),
    };
}