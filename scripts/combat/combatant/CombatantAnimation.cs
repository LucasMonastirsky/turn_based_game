namespace Combat {
    public partial class Combatant {
        protected CombatAnimator Animator;
        public abstract partial class StandardAnimationStore {
            public abstract SimpleAnimation Idle { get; set; }
            public abstract SimpleSprite Hurt { get; set; }
            public abstract SimpleSprite Parry { get; set; }
            public abstract SimpleSprite Dodge { get; set; }
            public abstract SimpleSprite Dead { get; set; } // PCs get knocked instead, and die the third time?
        }
        protected abstract StandardAnimationStore StandardAnimations { get; }

        public virtual void ResetAnimation () {
            if (!IsDead) Animator.Play(StandardAnimations.Idle);
            else Animator.Play(StandardAnimations.Dead);
        }
    }
}