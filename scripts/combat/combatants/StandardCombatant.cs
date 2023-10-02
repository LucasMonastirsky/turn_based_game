using System;
using CustomDebug;
using Godot;

namespace Combat {
    public abstract partial class StandardCombatant : Node2D, ICombatant {
        public IController Controller { get; protected set; }

        public abstract string CombatName { get; }
        [Export] public int Health { get; protected set; } = 50;
        [Export] public int Armor { get; protected set; }

        private Side _side;
        public Side Side {
            get => _side;
            set {
                _side = value;
                Animator.Flipped = value == Side.Right;
            }
        }

        public int Row { get; private set; }
        public int RowPos { get; private set; }

        #region Management
        public void LoadIn (Position position) {
            Side = position.Side;
            Row = position.Row;
            RowPos = position.RowPos;

            var world_pos = Battle.Current.Positioner.GetWorldPosition(position);
            Position = world_pos;
        }
        #endregion

        public void SwitchWith(ICombatant target) {
            throw new NotImplementedException();
        }

        public void SwitchTo(Position position) {
            throw new NotImplementedException();
        }

        public int Damage(int value, string[] tags) {
            var total = Math.Clamp(value - Armor, 0, 999);

            Health -= total;
            Dev.Log($"{CombatName} received {total} damage ({value} - {Armor}). Health: {Health}");

            Animator.Play(StandardAnimations.Hurt);
            InteractionManager.OnActionEnd(() => Animator.Play(StandardAnimations.Idle));

            return value;
        }

        public virtual void ReceiveAttack (AttackResult attack_result) {
            if (attack_result.Parried && attack_result.Dodged) OnAttackParriedAndDodged(attack_result);
            else if (attack_result.Parried) OnAttackParried(attack_result);
            else if (attack_result.Dodged) OnAttackDodged(attack_result);
        }

        protected virtual void OnAttackParried (AttackResult attack_result) {
            Animator.Play(StandardAnimations.Parry);
            InteractionManager.OnActionEnd(() => Animator.Play(StandardAnimations.Idle));
        }
        protected virtual void OnAttackDodged (AttackResult attack_result) {
            Animator.Play(StandardAnimations.Dodge);
            InteractionManager.OnActionEnd(() => Animator.Play(StandardAnimations.Idle));
        }
        protected virtual void OnAttackParriedAndDodged (AttackResult attack_result) {
            OnAttackParried(attack_result);
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
            public abstract CombatAnimation Parry { get; set; }
            public abstract CombatAnimation Dodge { get; set; }
        }
        protected abstract AnimationStore StandardAnimations { get; }

        public CombatAnimator.EventManager Play (CombatAnimation animation) {
            return Animator.Play(animation);
        }
        #endregion

        #region Godot
        public override void _Ready () {
            Animator = GetNode<CombatAnimator>("Animator");

            if (Animator == null) {
                Dev.Error("Animator is null");
                Animator = new CombatAnimator();
                AddChild(Animator);
                Dev.Log("New animator: " + Animator.GetPath());
            }

            Animator.Play(StandardAnimations.Idle);
        }
        #endregion
    }
}