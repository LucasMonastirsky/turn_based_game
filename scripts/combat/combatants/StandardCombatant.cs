using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomDebug;
using Godot;

namespace Combat {
    public abstract partial class StandardCombatant : Node2D, ICombatant {
        public Controller Controller { get; set; }

        public abstract string CombatName { get; }
        [Export] public int Health { get; protected set; } = 50;
        [Export] public int Armor { get; protected set; }

        private CombatPosition _combat_position;
        public CombatPosition CombatPosition {
            get => _combat_position;
            set {
                _combat_position = value;
                Animator.Flipped = _combat_position.Side == Side.Right;
            }
        }

        public Vector2 WorldPos { get => Position; }
        public abstract List<CombatAction> ActionList { get; }

        #region Management
        protected virtual void Setup () {

        }

        public void LoadIn (CombatPosition position) {
            CombatPosition = position;

            Setup();
        }

        public virtual void OnActionEnd () {
            Dev.Log($"OnActionEnd {CombatName}");
            Animator.Play(StandardAnimations.Idle);
        }
        #endregion

        #region Combat Interactions
        public void SwitchWith(ICombatant target) {
            throw new NotImplementedException();
        }

        public void SwitchTo(CombatPosition position) {
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

        private double movement_duration = Timing.MoveDuration; 
        private bool moving = false;
        private Vector2 moving_from, moving_towards;
        private double moving_time;
        private TaskCompletionSource move_completion_source;

        public Task MoveTo (Vector2 target_position) {
            move_completion_source = new TaskCompletionSource();
            moving = true;
            moving_from = Position;
            moving_towards = target_position;
            moving_time = 0;

            return move_completion_source.Task;
        }

        public Task MoveBack () {
            return MoveTo(Positioner.GetWorldPosition(CombatPosition));
        }

        public Task MoveToMelee (ICombatant target) {
            return MoveTo(target.WorldPos with { X = target.WorldPos.X + 50 * (int) CombatPosition.Side }); // TODO: put melee range var somewhere
        }
        #endregion

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
        public override void _Ready () {
            Animator = new NewCombatAnimator();
            AddChild(Animator);
            Animator.Play(StandardAnimations.Idle);
        }

        public override void _Process (double delta) {
            if (moving) { // TODO: this is kinda dirty, use callback list?
                moving_time += delta;
                Position = moving_from.Lerp(moving_towards, (float) (moving_time / movement_duration));
                if (moving_time >= movement_duration) {
                    moving = false;
                    move_completion_source.TrySetResult();
                }
            }
        }
        #endregion
    }
}