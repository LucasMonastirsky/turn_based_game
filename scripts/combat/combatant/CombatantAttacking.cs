using System;
using System.Threading.Tasks;
using Development;

namespace Combat {
    public partial class Combatant {
         public int Damage(int value, string[] tags) {
            var total = Math.Clamp(value, 0, 999);

            Health -= total;
            Dev.Log(Dev.TAG.COMBAT, $"{this} received {total} damage [{string.Join(",", tags)}]");

            Animator.Play(StandardAnimations.Hurt);

            return value;
        }

        public virtual void ReceiveAttack (AttackResult attack_result) {
            Dev.Log(Dev.TAG.COMBAT, $"{this} received attack - {attack_result}");
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
                    
        public struct BasicAttackOptions {
            public int ParryNegation, DodgeNegation;
        }

        public async Task<AttackResult> Attack (CombatTarget target, BasicAttackOptions options) {
            CombatEvents.BeforeAttack.Trigger(new () { Attacker = this, Target = target, Options = options });

            await InteractionManager.ResolveQueue();

            return target.Combatant.ReceiveAttack(this, options);
        }

        public AttackResult ReceiveAttack (Combatant attacker, BasicAttackOptions? options = null) {
            var hit_roll = attacker.Roll(new DiceRoll(10), new string[] { "Attack" });
            var parry_roll = Roll(new DiceRoll(10), new string[] { "Parry" });
            var dodge_roll = Roll(new DiceRoll(10), new string[] { "Dodge" });

            var result = new AttackResult {
                Attacker = attacker,
                Defender = this,
                ParryDelta = parry_roll.Total - hit_roll.Total - options?.ParryNegation ?? 0,
                DodgeDelta = dodge_roll.Total - hit_roll.Total - options?.DodgeNegation ?? 0,
            };

            if (result.Parried && result.Dodged) OnAttackParriedAndDodged(result);
            else if (result.Parried) OnAttackParried(result);
            else if (result.Dodged) OnAttackDodged(result);

            return result;
        }
    }
}