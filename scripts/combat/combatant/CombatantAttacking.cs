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

        public virtual async Task Riposte (AttackResult attack_result) {

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

        public async Task BasicAttack (CombatTarget target, BasicAttackOptions options, Func<AttackResult, Task> function) {
            CombatEvents.BeforeAttack.Trigger(new () { Attacker = this, Target = target, Options = options });
            await InteractionManager.ResolveQueue();

            var result = target.Combatant.ReceiveAttack(this, options);
            await function(result);

            if (TurnManager.ActiveCombatant != result.Defender) await result.Defender.Riposte(result);

            CombatEvents.AfterAttack.Trigger(new () { Attacker = this, Target = target, Options = options, Result = result });
            await InteractionManager.ResolveQueue();
        }

        public AttackResult ReceiveAttack (Combatant attacker, BasicAttackOptions? options = null) {
            var hit_roll = attacker.Roll(10, new string [] { "Attack" });
            var parry_roll = Roll(10, new string [] { "Parry" });
            var dodge_roll = Roll(10, new string [] { "Dodge" });

            var result = new AttackResult {
                Attacker = attacker,
                Defender = this,
                ParryDelta = parry_roll - hit_roll - options?.ParryNegation ?? 0,
                DodgeDelta = dodge_roll - hit_roll - options?.DodgeNegation ?? 0,
                AllowRiposte = false,
            };

            if (result.Parried && result.Dodged) OnAttackParriedAndDodged(result);
            else if (result.Parried) OnAttackParried(result);
            else if (result.Dodged) OnAttackDodged(result);

            return result;
        }
    }
}