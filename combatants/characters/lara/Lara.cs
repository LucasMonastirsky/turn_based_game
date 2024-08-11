using System;
using System.Threading.Tasks;
using Development;
using static Dice;

namespace Combat {
    public partial class Lara : Combatant {
        public override string Name => "Hidan";

        public override Type DefaultControllerType => typeof(PlayerController);

        public DiceRoll AxeDamageRoll = D6.Times(2).Plus(2);
        public DiceRoll PunchDamageRoll = D4.Times(2).Plus(1);

        protected override void Setup () {
            base.Setup();
            Actions = new (this);

            Health = 30;
            BaseMaxHealth = 30;

            BaseHitBonus = 2;
            BaseParryBonus = 1;
            BaseDodgeBonus = 3;
        }

        public override CombatAction GetRiposte(AttackResult attack_result) {
            if (attack_result.Hit) return Actions.Stab.Bind(attack_result.Attacker);
            else return null;
        }

        protected override void OnDamaged (int value) {
            AddStatusEffect(new Rage(value));
        }
    }
}