using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using Development;

namespace Combat {
    public abstract partial class Combatant : Node2D {
        public Controller Controller { get; set; }

        public abstract string CombatName { get; }
        [Export] public int Health { get; protected set; } = 50;
        [Export] public int Armor { get; protected set; }

        public bool IsDead { get; protected set; }

        private CombatPosition _combat_position;
        public CombatPosition CombatPosition {
            get => _combat_position;
            set {
                _combat_position = value;
                Animator.Flipped = _combat_position.Side == Side.Right;
            }
        }
        public int Slot { get => CombatPosition.Slot; }
        public int Row { get => CombatPosition.Row; }
        public Side Side { get => CombatPosition.Side; }

        public Vector2 WorldPos { get => Position; }
        public abstract List<CombatAction> ActionList { get; }

        #region Management
        protected virtual void Setup () {

        }

        public void LoadIn (CombatPosition position) {
            CombatPosition = position;

            Setup();
        }

        public virtual void OnPreActionEnd () {

        }

        /// <summary>
        ///  Don't add to resolve_queue here
        /// </summary>
        public virtual void OnActionEnd () {
            if (Health < 1) {
                if (!IsDead) {
                    Dev.Log(Dev.TAG.COMBAT, $"{CombatName} dead in {CombatPosition}");
                    Animator.Play(StandardAnimations.Dead);
                    IsDead = true;
                }
            }
            else {
                Animator.Play(StandardAnimations.Idle);
            }
        }
        #endregion

        #region Combat Interactions
        public void SwitchWith(Combatant target) { // remove?
            throw new NotImplementedException();
        }

        public void SwitchTo(CombatPosition position) {
            throw new NotImplementedException();
        }

        public int Damage(int value, string[] tags) {
            var total = Math.Clamp(value - Armor, 0, 999);

            Health -= total;
            Dev.Log(Dev.TAG.COMBAT, $"{CombatName} received {total} damage ({value} - {Armor}). Health: {Health}");

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

        private double movement_duration = (double) Timing.MoveDuration / 1000; 
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

        public Task MoveToMelee (Combatant target) {
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
        protected CombatAnimator Animator;
        public abstract partial class StandardAnimationStore {
            public abstract SimpleAnimation Idle { get; set; }
            public abstract SimpleSprite Hurt { get; set; }
            public abstract SimpleSprite Parry { get; set; }
            public abstract SimpleSprite Dodge { get; set; }
            public abstract SimpleSprite Dead { get; set; } // PCs get knocked instead, and die the third time?
        }
        protected abstract StandardAnimationStore StandardAnimations { get; }
        #endregion

        #region Godot
        public override void _Ready () {
            Animator = new CombatAnimator();
            AddChild(Animator);
            Animator.Play(StandardAnimations.Idle);
        }

        public override void _Process (double delta) {
            if (moving) { // TODO: this is kinda dirty, use callback list?
                moving_time += delta;
                Position = moving_from.Lerp(moving_towards, (float) (moving_time / movement_duration));
                if (moving_time >= movement_duration) {
                    Position = moving_towards;
                    moving = false;
                    move_completion_source.TrySetResult();
                }
            }
        }
        #endregion
    }
}