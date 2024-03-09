using System;

namespace Combat {
    public partial class Hidan : Combatant {
        public override string Name => "Hidan";

        public override Type DefaultControllerType => typeof(PlayerController);

        protected override void Setup () {
            base.Setup();
            Actions = new (this);
        }

        public override CombatAction GetRiposte(AttackResult attack_result) {
            if (attack_result.Hit) return Actions.Stab.Bind(attack_result.Attacker);
            else return null;
        }
    }
}