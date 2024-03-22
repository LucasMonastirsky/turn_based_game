using System.Linq;
using Development;
using Godot;

namespace Combat {
    public partial class Combatant {
        protected CombatAnimator Animator;

        public class StandardAnimationStore {
            public virtual SimpleAnimation Idle { get; set; }
            public virtual SimpleSprite Hurt { get; set; }
            public virtual SimpleSprite Parry { get; set; }
            public virtual SimpleSprite Dodge { get; set; }
            public virtual SimpleSprite Dead { get; set; } // PCs get knocked instead, and die the third time?
        }

        protected abstract StandardAnimationStore StandardAnimations { get; }

        public void Play (SimpleAnimation animation) {
            Animator.Play(animation);
        }

        public void Play (SimpleSprite sprite) {
            Animator.Play(sprite);
        }

        public void Play (AudioStream audio) {
            var audio_player = Node.AudioPlayers.FirstOrDefault(audio_player =>
                audio_player.Stream?.ResourceName == audio.ResourceName
                || !audio_player.Playing
            );

            if (audio_player is null) {
                // Dev.Error($"No audio player available in {this}");
            }
            else {
                audio_player.Stream = audio;
                audio_player.Play();
            }
        }

        public virtual void ResetAnimation () {
            if (!IsDead) Animator.Play(StandardAnimations.Idle);
            else Animator.Play(StandardAnimations.Dead);
        }
    }
}