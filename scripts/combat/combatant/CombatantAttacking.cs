using System;
using System.Threading.Tasks;
using Development;
using Godot;

namespace Combat {
    public partial class Combatant {

        public virtual bool CanParry => true;
        public virtual bool CanDodge => true;

        public virtual CombatAction GetRiposte (AttackResult attack_result) {
            return null;
        }

        protected virtual void OnAttackParried (AttackResult attack_result) {
            Animator.Play(StandardAnimations.Parry);
            Play(CommonSounds.SwordClash);
            DamageLabel.Instantiate(this, "Parry");
        }

        protected void OnAttackDodged (AttackResult attack_result) {
            Animator.Play(StandardAnimations.Dodge);
            Play(CommonSounds.Woosh);
            DamageLabel.Instantiate(this, "Dodge");
        }

        public async Task<AttackResult> SendAttack (Targetable targetable, Attack attack, Func<AttackResult, Task> handler = null) {
            attack.Attacker = this;
            attack.Target = targetable.ToTarget();

            if (attack.MoveToMeleeDistance) await DisplaceToMeleeDistance(attack.Target);

            await Events.BeforeAttack.Trigger(attack);
            await CombatEvents.BeforeAttack.Trigger(attack);

            var result = attack.Target.Combatant.ReceiveAttack(this, attack);

            if (result.Hit && !result.IsCrit && Roll(Dice.D20.Plus(attack.CritBonus), Stat.CritBonus) > 20) {
                result.IsCrit = true;
            }

            if (attack.OnResult != null) attack.OnResult(result);

            if (attack.Sprite != null) Play(attack.Sprite);
            if (attack.Sound != null) Play(attack.Sound);

            if (result.Hit && attack.DamageRoll != null) {
                if (result.IsCrit) {
                    Play(CommonSounds.Crit);
                    attack.DamageRoll = attack.DamageRoll.Times(2);
                }

                result.DamageDone = result.Defender.Damage(Roll(attack.DamageRoll, Stat.CritBonus), this);
            }

            if (handler != null) await handler(result);

            await CombatEvents.AfterAttack.Trigger(result);

            return TurnManager.LastAttack = result;
        }
        public AttackResult ReceiveAttack (Combatant attacker, Attack attack) {
            var hit_roll = attacker.Roll(Dice.D10.Plus(attack.HitBonus).WithAdvantage(attack.HitAdvantage), Stat.HitBonus);
            var parry_roll = (!attack.CanBeParried || IsDead || !CanParry) ? 0 : Roll(Dice.D10, Stat.ParryBonus);
            var dodge_roll = (!attack.CanBeDodged || IsDead || !CanMove || !CanDodge) ? 0 : Roll(Dice.D10, Stat.DodgeBonus);

            var result = new AttackResult {
                Attack = attack,
                Attacker = attacker,
                Defender = this,
                HitRoll = hit_roll,
                ParryRoll = parry_roll,
                DodgeRoll = dodge_roll,
                ParryNegation = attack.ParryNegation,
                DodgeNegation = attack.DodgeNegation,
                IsCrit = attack.IsCrit,
            };

            if (result.Parried) OnAttackParried(result);
            if (result.Dodged) OnAttackDodged(result);
            if (result.Missed) {
                if (Health > 0) Play(StandardAnimations.Idle);
                DamageLabel.Instantiate(this, "Miss");
            }

            var anti_parry = result.HitRoll + result.ParryNegation;
            var anti_dodge = result.HitRoll + result.DodgeNegation;
            Dev.Log(Dev.Tags.Combat, $"{result} P{result.ParryRoll}/{anti_parry} D{result.DodgeRoll}/{anti_dodge}");

            return result;
        }
    }
}