using System;
using System.Threading.Tasks;
using Development;
using Utils;

namespace Combat {
    public partial class Combatant {
         public int Damage(int value, string[] tags) {
            if (!IsDead) Animator.Play(StandardAnimations.Hurt);

            var total = Math.Clamp(value, 0, 999);

            var previous_health = Health;
            Health -= total;
            Dev.Log(Dev.Tags.Combat, $"{this} received {total} damage {Stringer.Join(tags)}");

            if (previous_health > 0 && IsDead) CombatEvents.AfterDeath.Trigger(new () { Combatant = this });

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
                    
        public class BasicAttackOptions {
            public int ParryNegation, DodgeNegation;
            public string [] HitRollTags;

            public BasicAttackOptions () {
                (ParryNegation, DodgeNegation) = (0, 0);
                HitRollTags = new string [] {};
            }
        }

        public async Task BasicAttack (CombatTarget target, BasicAttackOptions options, Func<AttackResult, Task> function) {
            CombatEvents.BeforeAttack.Trigger(new () { Attacker = this, Target = target, Options = options });
            await InteractionManager.ResolveQueue();

            var result = target.Combatant.ReceiveAttack(this, options);
            await function(result);

            CombatEvents.AfterAttack.Trigger(new () { Attacker = this, Target = target, Options = options, Result = result });
            TurnManager.LastAttack = result;

            await InteractionManager.ResolveQueue();
        }

        public AttackResult ReceiveAttack (Combatant attacker, BasicAttackOptions options) {
            var hit_roll = attacker.Roll(10, options.HitRollTags);
            var parry_roll = IsDead ? 0 : Roll(10, new string [] { "Parry" });
            var dodge_roll = IsDead ? 0 : Roll(10, new string [] { "Dodge" });

            var result = new AttackResult {
                Attacker = attacker,
                Defender = this,
                ParryDelta = parry_roll - hit_roll - options.ParryNegation,
                DodgeDelta = dodge_roll - hit_roll - options.DodgeNegation,
                AllowRiposte = false,
            };

            if (result.Parried && result.Dodged) OnAttackParriedAndDodged(result);
            else if (result.Parried) OnAttackParried(result);
            else if (result.Dodged) OnAttackDodged(result);

            return result;
        }
    }
}