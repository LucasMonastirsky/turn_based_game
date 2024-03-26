using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Development;
using Godot;
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

        public record AttackOptions {

            public int ParryNegation, DodgeNegation = 0;
            public List<RollModifier> RollModifiers = new ();
            public DiceRoll DamageRoll = null;
            public string [] DamageTags = new string [] {};
            public bool IsMelee = false;
            public bool IsRanged = false;
            public bool MoveToMeleeDistance = false;
            public SimpleSprite Sprite = null;
            public AudioStream Sound = null;
        }

        public async Task<AttackResult> Attack (Targetable targetable, AttackOptions options, Func<AttackResult, Task> handler = null) {
            var target = targetable.ToTarget();

            if (options.MoveToMeleeDistance) await DisplaceToMeleeDistance(target);

            await CombatEvents.BeforeAttack.Trigger(new () { Attacker = this, Target = target, Options = options });

            var result = target.Combatant.ReceiveAttack(this, options);

            if (options.Sprite != null) Play(options.Sprite);
            if (options.Sound != null) Play(options.Sound);

            if (result.Hit && options.DamageRoll != null) {
                var crit_roll = Roll(Dice.D20, RollTags.Crit);

                if (crit_roll > 20 - CritSensitivity) {
                    Play(CommonSounds.Crit);
                    options.DamageRoll.Times(2);
                }

                result.Defender.Damage(Roll(options.DamageRoll), options.DamageTags.Prepend("Damage").ToArray());
            }

            if (handler != null) await handler(result);

            await CombatEvents.AfterAttack.Trigger(new () { Attacker = this, Options = options, Result = result, Target = result.Defender.ToTarget() });

            return TurnManager.LastAttack = result;
        }
        public AttackResult ReceiveAttack (Combatant attacker, AttackOptions options) {
            var hit_roll = attacker.Roll(10, options.RollModifiers, RollTags.Attack, RollTags.Hit);
            var parry_roll = IsDead ? 0 : Roll(10, RollTags.Defense, RollTags.Parry);
            var dodge_roll = (IsDead || !CanMove) ? 0 : Roll(10, RollTags.Defense, RollTags.Dodge);

            var result = new AttackResult {
                Attacker = attacker,
                Defender = this,
                HitRoll = hit_roll,
                ParryRoll = parry_roll,
                DodgeRoll = dodge_roll,
                ParryNegation = options.ParryNegation,
                DodgeNegation = options.DodgeNegation,
            };

            if (result.Parried) OnAttackParried(result);
            if (result.Dodged) OnAttackDodged(result);
            if (result.Missed && IsAlive) Play(StandardAnimations.Idle);

            Dev.Log(Dev.Tags.Combat, $"{result}");

            return result;
        }
    }
}