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

        public virtual void ResetAnimation () {
            if (!IsDead) Animator.Play(StandardAnimations.Idle);
            else Animator.Play(StandardAnimations.Dead);
        }
    }
}