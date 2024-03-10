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

        public class TheShakes : StatusEffect {
            public override string Name => "The Shakes";
            public override bool Stackable => true;

            private int _level;
            public override int Level {
                get => _level;
                set {
                    _level = value;
                    RollModifier.Bonus = -value;
                }
            }

            public RollModifier RollModifier { get; private set; }

            public override void OnApplied() {
                RollModifier = new (this, "Attack", "Shot");
                User.AddRollModifier(RollModifier);
                Level = 1;
            }

            public override void Stack (StatusEffect new_effect) {
                Level++;
            }
        }
    }
}