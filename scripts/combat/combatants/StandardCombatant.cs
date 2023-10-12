using System;
using CustomDebug;
using Godot;

namespace Combat {
    public abstract partial class StandardCombatant : Node2D, ICombatant {
        public Controller Controller { get; set; }

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
        public Vector2 WorldPos { get => Position; }

        #region Management
        public void LoadIn (Position position) {
            Side = position.Side;
            Row = position.Row;
            RowPos = position.RowPos;

            var world_pos = Battle.Current.Positioner.GetWorldPosition(position);
            Position = world_pos;
        }

        public virtual void OnActionEnd () {
            Dev.Log($"OnActionEnd {CombatName}");
            Animator.Play(StandardAnimations.Idle);
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

            return value;
        }

        public virtual void ReceiveAttack (AttackResult attack_result) {
            if (attack_result.Parried && attack_result.Dodged) OnAttackParriedAndDodged(attack_result);
            else if (attack_result.Parried) OnAttackParried(attack_result);
            else if (attack_result.Dodged) OnAttackDodged(attack_result);
        }

        protected virtual void OnAttackParried (AttackResult attack_result) {
            Animator.Play(StandardAnimations.Parry);
        }
        protected virtual void OnAttackDodged (AttackResult attack_result) {
            Animator.Play(StandardAnimations.Dodge);
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
        protected NewCombatAnimator Animator;
        public abstract partial class StandardAnimationStore {
            public abstract SimpleAnimation Idle { get; set; }
            public abstract SimpleSprite Hurt { get; set; }
            public abstract SimpleSprite Parry { get; set; }
            public abstract SimpleSprite Dodge { get; set; }
        }
        protected abstract StandardAnimationStore StandardAnimations { get; }
        #endregion

        #region Godot
        protected virtual void Initialize () {}
        public override void _Ready () {
            Animator = new NewCombatAnimator();
            AddChild(Animator);
            Animator.Play(StandardAnimations.Idle);

            Initialize();
        }
        #endregion
    }
}