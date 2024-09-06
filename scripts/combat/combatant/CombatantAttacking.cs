using System;
using System.Linq;
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
            Animator.Play(Animations.Parry);
            Play(CommonSounds.SwordClash);
            DamageLabel.Instantiate(this, "Parry");
        }

        protected void OnAttackDodged (AttackResult attack_result) {
            Animator.Play(Animations.Dodge);
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

            if (result.Hit && !result.IsCrit && Roll(Dice.D20.Plus(attack.CritBonus), Stat.Crit) > 20) {
                result.IsCrit = true;
            }

            if (attack.OnResult != null) attack.OnResult(result);

            if (attack.Sprite != null) Play(attack.Sprite);
            if (attack.Sound != null) Play(attack.Sound);

            if (result.Hit) {
                if (result.IsCrit) {
                    Play(CommonSounds.Crit);
                    attack.DamageAmount *= attack.CritMultiplier;
                }

                result.DamageDone = SendDamage(result.Defender, attack.DamageAmount, attack.DamageDeviation, attack.IsCrit).TotalDealt;
                if (result.DamageDone > 0) Play(attack.HitSound ?? CommonSounds.SwordWound);
            }

            if (handler != null) await handler(result);

            await CombatEvents.AfterAttack.Trigger(result);

            return TurnManager.LastAttack = result;
        }
        public AttackResult ReceiveAttack (Combatant attacker, Attack attack) {
            var hit_roll = attacker.Roll(Dice.D10.Plus(attack.HitBonus), Stat.Hit);

            var parry_roll = (!attack.CanBeParried || IsDead || !CanParry) ? 0 : Roll(Dice.D10, Stat.Parry);
            var dodge_roll = (!attack.CanBeDodged || IsDead || !CanMove || !CanDodge) ? 0 : Roll(Dice.D10, Stat.Dodge);

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
                if (Health > 0) Play(Animations.Idle);
                DamageLabel.Instantiate(this, "Miss");
            }

            var anti_parry = result.HitRoll + result.ParryNegation;
            var anti_dodge = result.HitRoll + result.DodgeNegation;
            Dev.Log(Dev.Tags.Combat, $"{result} P{result.ParryRoll}/{anti_parry} D{result.DodgeRoll}/{anti_dodge}");

            return result;
        }
    }
}