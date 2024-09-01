using Combat;
using Godot;

public partial class ShieldGuy {
    private static string texture_path = "res://combatants/enemies/shield_guy/textures";

    public class AnimationStore : StandardAnimationStore {
        public SimpleSprite Stab, Throw, ShieldBlock;
    }

    public override StandardAnimationStore StandardAnimations => Animations;

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

    public SoundStore Sounds = new ();

    public class SoundStore {
        public AudioStream Bash;
        public AudioStream ShieldHit;

        public SoundStore () {
            Bash = GD.Load<AudioStream>("res://combatants/enemies/shield_guy/sounds/bash.wav");
            ShieldHit = GD.Load<AudioStream>("res://combatants/enemies/shield_guy/sounds/shield_hit.wav");
        }
    }
}