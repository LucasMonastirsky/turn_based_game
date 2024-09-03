using Combat;
using Godot;

public partial class ShieldGuy {
    public override string resources_path => "res://combatants/enemies/shield_guy/resources";

    public class AnimationStore : StandardAnimationStore {
        public SimpleSprite Stab, Throw, ShieldBlock;
    }

    public override AnimationStore Animations => _animations;
    private AnimationStore _animations;

    protected override void LoadSprites () {
        _animations = new () {
            Idle = new SimpleAnimation () {
                Sprites = new SimpleSprite [] {
                    LoadSprite("idle_0"),
                    LoadSprite("idle_1"),
                },
            },
            Hurt = LoadSprite("hurt"),
            Parry = LoadSprite("parry"),
            Dodge = LoadSprite("dodge"),
            Stab = LoadSprite("stab"),
            Throw = LoadSprite("throw"),
            ShieldBlock = LoadSprite("shield_block"),
        };
    }

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