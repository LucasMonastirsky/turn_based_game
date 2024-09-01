using Godot;

namespace Combat {
    public partial class Lara {
        private static string texture_path = "res://combatants/characters/lara/textures";

        public override StandardAnimationStore StandardAnimations => Animations;

        public class AnimationStore : StandardAnimationStore {
            public SimpleSprite Stab, Charge, Punch, Push;
            public SimpleSprite [] Sweeps;
        }

        public AnimationStore Animations = new () {
            Idle = new SimpleAnimation (new []{
                new SimpleSprite (texture_path, "idle_0"),
                new SimpleSprite (texture_path, "idle_1"),
            }),
            Hurt = new (texture_path, "hurt"),
            Parry = new (texture_path, "parry"),
            Dodge = new (texture_path, "dodge"),
            Dead = new (texture_path, "dead"),
            Stab = new (texture_path, "stab"),
            Charge = new (texture_path, "charge"),
            Punch = new (texture_path, "punch"),
            Push = new (texture_path, "push"),
            Sweeps = new SimpleSprite [] {
                new (texture_path, "sweep_0"),
                new (texture_path, "sweep_1"),
            },
        };
    }
}