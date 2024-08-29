using System;
using Development;

namespace Combat {
    public partial class Combatant {

        public int Damage (int value, Combatant sender, bool is_crit = false) {
            var damage_instance = new DamageInstance () {
                Sender = sender,
                Receiver = this,
                Amount = value,
                IsCrit = is_crit,
            };

            CombatEvents.BeforeDamage.Trigger(damage_instance);

            if (!is_crit) {
                damage_instance.Amount -= Armor;
                if (damage_instance.Amount < 0) damage_instance.Amount = 0;
            }
            
            var amount = damage_instance.Amount;
            var previous_total_health = TotalHealth;
            
            if (ExtraHealth > 0) {
                if ((ExtraHealth -= amount) < 0) {
                    Health -= -ExtraHealth;
                    ExtraHealth = 0;
                }
            }
            else {
                Health -= amount;
            }

            Dev.Log(Dev.Tags.Combat, $"{this} received {amount} damage");

            if (previous_total_health > 0 && Health < 1) {
                if (DeathEvent != null) InteractionManager.AddQueueEvent(DeathEvent);
            }

            if (!IsDead) Animator.Play(StandardAnimations.Hurt);
            Play(CommonSounds.SwordWound);

            DamageLabel.Instantiate(this, $"{amount}");

            CombatEvents.AfterDamage.Trigger(damage_instance);

            return value;
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