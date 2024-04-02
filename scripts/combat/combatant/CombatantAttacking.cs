using System;
using System.Threading.Tasks;
using Development;
using Godot;

namespace Combat {
    public partial class Combatant {

        public virtual bool CanParry => true;
        public virtual bool CanDodge => true;

        public int Damage (int value, bool is_crit = false) {
            if (!IsDead) Animator.Play(StandardAnimations.Hurt);

            var total = Math.Clamp(value - (is_crit ? Armor : 0), 1, 999);

            var previous_health = Health;
            Health -= total;

            Dev.Log(Dev.Tags.Combat, $"{this} received {total} damage");

            if (previous_health > 0 && Health < 1) {
                if (DeathEvent != null) InteractionManager.AddQueueEvent(DeathEvent);
            }

            Play(CommonSounds.SwordWound);

            DamageLabel.Instantiate(this, $"{value}");

            OnDamaged(value);

            return value;
        }

        protected virtual void OnDamaged (int value) {

        }

        public virtual CombatAction GetRiposte (AttackResult attack_result) {
            return null;
        }

        protected void OnAttackParried (AttackResult attack_result) {
            Animator.Play(StandardAnimations.Parry);
            Play(CommonSounds.SwordClash);
            DamageLabel.Instantiate(this, "Parry");
        }

        protected void OnAttackDodged (AttackResult attack_result) {
            Animator.Play(StandardAnimations.Dodge);
            Play(CommonSounds.Woosh);
            DamageLabel.Instantiate(this, "Dodge");
        }

        public record Attack {
            public Combatant Attacker;
            public CombatTarget Target;
            public int HitAdvantage, HitBonus = 0;
            public int ParryNegation, DodgeNegation = 0;
            public bool CanBeParried = true;
            public bool CanBeDodged = true;
            public DiceRoll DamageRoll = null;
            public bool IsCrit = false;
            public bool IsMelee = false;
            public bool IsRanged = false;
            public bool MoveToMeleeDistance = false;
            public SimpleSprite Sprite = null;
            public AudioStream Sound = null;
        }

        public async Task<AttackResult> SendAttack (Targetable targetable, Attack attack, Func<AttackResult, Task> handler = null) {
            attack.Attacker = this;
            attack.Target = targetable.ToTarget();

            if (attack.MoveToMeleeDistance) await DisplaceToMeleeDistance(attack.Target);

            if (Roll(Dice.D20, RollTags.Crit) > 20) {
                Play(CommonSounds.Crit);
                attack.IsCrit = true;
            }

            await CombatEvents.BeforeAttack.Trigger(attack);

            var result = attack.Target.Combatant.ReceiveAttack(this, attack);

            if (attack.Sprite != null) Play(attack.Sprite);
            if (attack.Sound != null) Play(attack.Sound);

            if (result.Hit && attack.DamageRoll != null) {
                result.Defender.Damage(Roll(attack.DamageRoll, RollTags.Damage));
            }

            if (handler != null) await handler(result);

            await CombatEvents.AfterAttack.Trigger(new () { Attacker = this, Options = attack, Result = result, Target = result.Defender.ToTarget() });

            return TurnManager.LastAttack = result;
        }
        public AttackResult ReceiveAttack (Combatant attacker, Attack attack) {
            var hit_roll = attacker.Roll(Dice.D10, RollTags.Attack, RollTags.Hit);
            var parry_roll = (!attack.CanBeParried || IsDead || !CanParry) ? 0 : Roll(Dice.D10, RollTags.Defense, RollTags.Parry);
            var dodge_roll = (!attack.CanBeDodged || IsDead || !CanMove || !CanDodge) ? 0 : Roll(Dice.D10, RollTags.Defense, RollTags.Dodge);

            var result = new AttackResult {
                Attacker = attacker,
                Defender = this,
                HitRoll = hit_roll,
                ParryRoll = parry_roll,
                DodgeRoll = dodge_roll,
                ParryNegation = attack.ParryNegation,
                DodgeNegation = attack.DodgeNegation,
            };

            if (result.Parried) OnAttackParried(result);
            if (result.Dodged) OnAttackDodged(result);
            if (result.Missed) {
                if (Health > 0) Play(StandardAnimations.Idle);
                DamageLabel.Instantiate(this, "Miss");
            }

            Dev.Log(Dev.Tags.Combat, $"{result}");

            return result;
        }
    }
}