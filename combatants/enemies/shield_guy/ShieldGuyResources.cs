using Combat;

public partial class ShieldGuy {
    private static string texture_path = "res://combatants/enemies/shield_guy/textures";

    public class AnimationStore : StandardAnimationStore {
        public SimpleSprite Stab, Throw, ShieldBlock;
    }

    protected override StandardAnimationStore StandardAnimations => Animations;

    public AnimationStore Animations = new () {
        Idle = new SimpleAnimation () {
            Sprites = new SimpleSprite [] {
                new (texture_path, "idle_0"),
                new (texture_path, "idle_1"),
            },
        },
        Hurt = new (texture_path, "hurt"),
        Parry = new (texture_path, "parry"),
        Dodge = new (texture_path, "dodge"),
        Stab = new (texture_path, "stab"),
        Throw = new (texture_path, "throw"),
        ShieldBlock = new (texture_path, "shield_block"),
    };
}