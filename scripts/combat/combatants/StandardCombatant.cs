using System;
using CustomDebug;
using Godot;

namespace Combat {
    public abstract partial class StandardCombatant : Node2D, ICombatant {
        public IController Controller { get; protected set; }

        public abstract string CombatName { get; }
        [Export] public int Health { get; protected set; }
        [Export] public int Armor { get; protected set; }

        public Side Side { get; set; }
        public Row Row { get; set; }

        public CombatAnimator.EventManager Play (CombatAnimation animation) {
            return Animator.Play(animation);
        }

        public int Damage(int value, string[] tags) {
            var total = Math.Clamp(value - Armor, 0, double.PositiveInfinity);

            Dev.Log($"{CombatName} received {total} damage ({value} - {Armor})");

            Animator.Play(StandardAnimations.Hurt)
            .OnEnd(() => { Animator.Play(StandardAnimations.Idle); });

            return value;
        }

        #region Roll
        private RollManager roll_manager = new RollManager();

        public void AddPreRollEvent (string id, IRoller.PreRollCallback callback) {
            roll_manager.AddPreRollEvent(id, callback);
        }
        public void AddPreRollEvent (string id, string tag, IRoller.PreRollCallback callback) {
            roll_manager.AddPreRollEvent(id, tag, callback);
        }
        public void AddPreRollEvent (string id, string[] tags, IRoller.PreRollCallback callback) {
            roll_manager.AddPreRollEvent(id, tags, callback);
        }

        public void RemovePreRollEvent (string id) {
            roll_manager.RemovePreRollEvent(id);
        }

        public Roll.Result Roll (DiceRoll dice_roll, string[] tags) {
            return roll_manager.Roll(dice_roll, tags);
        }

        public Roll.Result Roll (DiceRoll[] dice_rolls, string[] tags) {
            return roll_manager.Roll(dice_rolls, tags);
        }
        #endregion

        #region Animation
        protected CombatAnimator Animator;
        public abstract partial class AnimationStore : Resource {
            public abstract CombatAnimation Idle { get; set; }
            public abstract CombatAnimation Hurt { get; set; }
        }
        protected abstract AnimationStore StandardAnimations { get; }
        #endregion

        #region Godot
        public override void _Ready () {
            Animator = GetNode<CombatAnimator>("Animator");

            if (Animator == null) {
                Dev.Error("Animator is null");
            }

            Animator.Play(StandardAnimations.Idle);
        }
        #endregion
    }
}