using System;

namespace Combat {
    public partial class Isabel : Combatant {
        public override string Name => "Isabel";

        public override Type DefaultControllerType => typeof(PlayerController);

        public Isabel () {
            Actions = new (this);

            BaseMaxHealth = 20;

            BaseDamageBonus = 0;
            BaseHitBonus = 3;
            BaseCritBonus = 5;
            BaseParryBonus = 3;
            BaseDodgeBonus = 7;

            CombatEvents.BeforeDamage.Always(async damage_instance => {
                if (damage_instance.Sender == this && damage_instance.Receiver.TotalHealth >= damage_instance.Receiver.MaxHealth) {
                    damage_instance.IsCrit = true;
                    damage_instance.Amount *= 2;
                } 
            });
        }

        public override CombatAction GetRiposte (AttackResult attack_result) {
            if (attack_result.Dodged) return Actions.Swing.Bind(attack_result.Attacker);
            else return null;
        }
    }
}