using Godot;

namespace Combat {
    public partial class Lara {
        public override string resources_path => "res://combatants/characters/lara/resources";

        public override AnimationStore Animations => _animations;
        private AnimationStore _animations;

        public class AnimationStore : StandardAnimationStore {
            public SimpleSprite Stab, Charge, Punch, Push;
            public SimpleSprite [] Sweeps;
        }

        protected override void LoadSprites () {
            _animations = new () {
                Idle = new SimpleAnimation (new []{
                    LoadSprite("idle_0"),
                    LoadSprite("idle_1"),
                }),
                Hurt = LoadSprite("hurt"),
                Parry = LoadSprite("parry"),
                Dodge = LoadSprite("dodge"),
                Stab = LoadSprite("stab"),
                Charge = LoadSprite("charge"),
                Punch = LoadSprite("punch"),
                Push = LoadSprite("push"),
                Sweeps = new SimpleSprite [] {
                    LoadSprite("sweep_0"),
                    LoadSprite("sweep_1"),
                },
            };
        }
    }
}