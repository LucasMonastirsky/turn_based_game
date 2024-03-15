using System;

namespace Combat {
    public partial class Anna : Combatant {
        public override string Name => $"Anna {Bullets}/{MaxBullets}";

        public override Type DefaultControllerType => typeof (PlayerController);

        public int Bullets = 6;
        public int MaxBullets = 6;

        protected override void Setup () {
            base.Setup();
            Actions = new (this);
            Tempo = 4;
        }

        public override CombatAction GetRiposte (AttackResult attack_result) {
            if (attack_result.Dodged) {
                return Actions.Kick.Bind(attack_result.Attacker);
            }

            return null;
        }

        public override void OnTurnEnd () {
            AddStatusEffect(new TheShakes ());
        }
    }
}