using System;

namespace Combat {
    public partial class Joseph : Combatant {
        public override string Name => "Joseph";

        public override Type DefaultControllerType => typeof (PlayerController);

        public Joseph () {
            Actions = new (this);

            BaseMaxHealth = 20;

            BaseHitBonus = 6;
            BaseCritBonus = 2;
            BaseParryBonus = 6;
            BaseDodgeBonus = 3;

            Passives = new () {
                new Frontliner (this),
                new Study (this),
            };
        }

        public override CombatAction GetRiposte (AttackResult attack_result) {
            if (attack_result.Parried && attack_result.Attack.IsMelee) return Actions.Swing.Bind(attack_result.Attacker);
            else return null;
        }
    }
}