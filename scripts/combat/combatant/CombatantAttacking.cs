using System;
using System.Collections.Generic;
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

            Play(CommonSounds.SwordWound); // TODO: add condition via tags

            OnDamaged(ref value, tags);

            return value;
        }

        protected virtual void OnDamaged (ref int value, string [] tags) {

        }

        public virtual CombatAction GetRiposte (AttackResult attack_result) {
            return null;
        }

        protected virtual void OnAttackParried (AttackResult attack_result) {
            Animator.Play(StandardAnimations.Parry);
            Play(CommonSounds.SwordClash);
        }
        protected virtual void OnAttackDodged (AttackResult attack_result) {
            Animator.Play(StandardAnimations.Dodge);
            Play(CommonSounds.Woosh);
        }
        protected virtual void OnAttackParriedAndDodged (AttackResult attack_result) {
            if (attack_result.ParryDelta > attack_result.DodgeDelta) OnAttackParried(attack_result);
            else OnAttackDodged(attack_result);
        }
                    
        public class BasicAttackOptions {
            public int ParryNegation, DodgeNegation;
            public string [] HitRollTags;
            public List<RollModifier> HitRollModifiers;

            public BasicAttackOptions () {
                (ParryNegation, DodgeNegation) = (0, 0);
                HitRollTags = new string [] {};
                HitRollModifiers = new ();
            }
        }

        public async Task<AttackResult> Attack (CombatTarget target, BasicAttackOptions options, Func<AttackResult, Task> function) {
            await CombatEvents.BeforeAttack.Trigger(new () { Attacker = this, Target = target, Options = options });

            var result = target.Combatant.ReceiveAttack(this, options);

            await function(result);

            await CombatEvents.AfterAttack.Trigger(new () { Attacker = result.Attacker, Options = options, Result = result, Target = result.Defender.ToTarget() });

            TurnManager.LastAttack = result;
            return result;
        }

        public async Task<AttackResult> Attack (CombatTarget target, BasicAttackOptions options) {
            await CombatEvents.BeforeAttack.Trigger(new () { Attacker = this, Target = target, Options = options });

            var result = target.Combatant.ReceiveAttack(this, options);

            await CombatEvents.AfterAttack.Trigger(new () { Attacker = result.Attacker, Options = options, Result = result, Target = result.Defender.ToTarget() });

            return TurnManager.LastAttack = result;
        }

        public AttackResult ReceiveAttack (Combatant attacker, BasicAttackOptions options) {
            var hit_roll = attacker.Roll(10, options.HitRollModifiers, options.HitRollTags);
            var parry_roll = IsDead ? 0 : Roll(10, new string [] { "Parry" });
            var dodge_roll = (IsDead || !CanMove) ? 0 : Roll(10, new string [] { "Dodge" });

            var result = new AttackResult {
                Attacker = attacker,
                Defender = this,
                HitRoll = hit_roll,
                ParryRoll = parry_roll,
                DodgeRoll = dodge_roll,
                ParryNegation = options.ParryNegation,
                DodgeNegation = options.DodgeNegation,
            };

            if (result.Parried && result.Dodged) OnAttackParriedAndDodged(result);
            else if (result.Parried) OnAttackParried(result);
            else if (result.Dodged) OnAttackDodged(result);
            else if (result.Missed && IsAlive) Play(StandardAnimations.Idle);

            Dev.Log(Dev.Tags.Combat, $"{result}");

            return result;
        }
    }
}