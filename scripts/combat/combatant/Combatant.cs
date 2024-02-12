using System;
using System.Collections.Generic;
using Godot;
using Development;

namespace Combat {
    public abstract partial class Combatant : Node2D, Targetable {
        public abstract string CombatName { get; }
        [Export] public int MaxHealth { get; protected set; } = 30;
        [Export] public int Health { get; protected set; } = 30;
        [Export] public int Armor { get; protected set; }

        public bool IsDead { get; protected set; }

        public abstract List<CombatAction> ActionList { get; }

        public CombatTarget ToTarget () => new CombatTarget (this);

        #region Combat Interactions
        public int Damage(int value, string[] tags) {
            var total = Math.Clamp(value - Armor, 0, 999);

            Health -= total;
            Dev.Log(Dev.TAG.COMBAT, $"{CombatName} received {total} damage ({value} - {Armor}). Health: {Health}");

            Animator.Play(StandardAnimations.Hurt);

            return value;
        }

        public virtual void ReceiveAttack (AttackResult attack_result) {
            Dev.Log(Dev.TAG.COMBAT, $"{CombatName} received attack - {attack_result}");

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
            OnAttackDodged(attack_result);
        }
        #endregion

        #region Status Effects
        public List<StatusEffect> StatusEffects { get; } = new ();

        public void AddStatusEffect (StatusEffect effect) {
            StatusEffects.Add(effect);
            effect.User = this;
            effect.OnApplied();
            Dev.Log(Dev.TAG.COMBAT, $"{CombatName} got status effect {effect.Name}");
        }
        #endregion

        #region Rolls
        protected RollManager roller = new RollManager();
        public Roll.Result Roll (DiceRoll roll, string[] tags) {
            return roller.Roll(roll, tags);
        }
        public Roll.Result Roll (DiceRoll[] rolls, string[] tags) {
            return roller.Roll(rolls, tags);
        }
        #endregion

        #region Godot
        public override void _Ready () {
            Animator = new CombatAnimator();
            AddChild(Animator);
            Animator.Play(StandardAnimations.Idle);
        }

        public override void _Process (double delta) {
            ProcessMovement(delta);
        }
        #endregion
    }
}