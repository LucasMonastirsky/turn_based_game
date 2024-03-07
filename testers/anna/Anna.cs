using System;
using System.Threading.Tasks;

namespace Combat {
    public partial class Anna : Combatant {
        public override string Name => $"Anna {Bullets}/{MaxBullets}";

        public override Type DefaultControllerType => typeof (PlayerController);

        public int Bullets = 6;
        public int MaxBullets = 6;

        protected override void Setup () {
            base.Setup();
            Actions = new (this);
        }

        public override async Task Riposte (AttackResult attack_result) {
            if (!attack_result.Hit) {
                InteractionManager.QueueAction(Actions.Kick.Bind(attack_result.Attacker));
            }
        }
    }
}