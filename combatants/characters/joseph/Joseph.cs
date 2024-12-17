using System;

namespace Combat {
    public partial class Joseph : Combatant {
        public override string Name => "Joseph";

        public override Type DefaultControllerType => typeof (PlayerController);

        public int HalberdDamage = 8;

        public Joseph () {
            Actions = new (this);

            BaseMaxHealth = 50;

            BaseDamageBonus = 2;
            BaseHitBonus = 6;
            BaseCritBonus = 1;
            BaseParryBonus = 6;
            BaseDodgeBonus = 3;

            Passives = new () {
                new Frontliner (this),
                new Study (this),
            };

            CombatEvents.AfterAttack.Always(async attack_result => {
                if (attack_result.Attacker == this && attack_result.Parried) {
                    InteractionManager.AddQueueEvent(async () => {
                        await Actions.ButtEnd.Bind(attack_result.Defender).Act();
                    });
                }
            });
        }

        public override CombatAction GetRiposte (AttackResult attack_result) {
            if (attack_result.Parried && attack_result.Attack.IsMelee) return Actions.Zornhau.Bind(attack_result.Attacker);
            else return null;
        }
    }
}