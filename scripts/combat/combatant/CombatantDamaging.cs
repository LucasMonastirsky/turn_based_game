using Development;
using Utils;

namespace Combat {
    public partial class Combatant {
        public static class Deviation {
            public static float Low = 0.1f;
            public static float Mid = 0.25f;
            public static float High = 0.5f;
        }

        public Damage SendDamage (
            Combatant receiver, int value, float deviation, bool is_crit = false, bool is_direct = true
        ) {
            var rolled_amount = RNG.Deviate(value, deviation) + DamageBonus;

            var damage = new Damage () {
                Amount = rolled_amount, IsCrit = is_crit, IsDirect = is_direct,
                Sender = this, Receiver = receiver,
            };

            return receiver.ReceiveDamage(damage);
        }

        public Damage ReceiveDamage (Damage damage) {
            CombatEvents.BeforeDamage.Trigger(damage);

            var damage_amount = damage.Amount;

            if (!damage.IsCrit) {
                damage_amount -= Armor;
                if (damage_amount < 0) damage_amount = 0;
            }
            
            var previous_total_health = TotalHealth;
            
            if (ExtraHealth > 0) {
                if ((ExtraHealth -= damage_amount) < 0) {
                    Health -= -ExtraHealth;
                    ExtraHealth = 0;
                }
            }
            else {
                Health -= damage_amount;
            }

            damage.TotalDealt = damage_amount;
            Dev.Log(Dev.Tags.Combat, $"{this} received {damage_amount} damage");

            if (previous_total_health > 0 && Health < 1) {
                if (DeathEvent != null) InteractionManager.AddQueueEvent(DeathEvent);
            }

            Animator.Play(Animations.Hurt);
            if (damage.IsCrit) Play(CommonSounds.Crit);
            DamageLabel.Instantiate(this, $"{damage.Amount}");
            

            CombatEvents.AfterDamage.Trigger(damage);

            return damage;
        }

        public int Heal (int value) {
            int sum = Health + ExtraHealth + value;

            if (sum > MaxHealth) value = MaxHealth - TotalHealth;
  
            ExtraHealth += value;

            DamageLabel.Instantiate(this, $"+{value}");

            return value;
        }

    }
}