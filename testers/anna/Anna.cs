using System;
using System.Threading.Tasks;
using Development;

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

        public override void OnTurnEnd () {
            AddStatusEffect(new TheShakes ());
        }

        public class TheShakes : StatusEffect {
            public override string Name => "The Shakes";
            public override bool Stackable => true;

            public RollModifier RollModifier { get; private set; }

            public override void OnApplied() {
                Level = 1;
                RollModifier = new (this, "Attack", "Shot") { Bonus = -Level };
                User.AddRollModifier(RollModifier);
            }

            public override void Stack (StatusEffect new_effect) {
                Level++;
                RollModifier.Bonus = -Level;
            }
        }
    }
}