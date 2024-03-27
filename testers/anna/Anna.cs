using System;
using System.Linq;
using static Dice;

namespace Combat {
    public partial class Anna : Combatant {
        public override string Name => $"Anna {Bullets}/{MaxBullets}";

        public override Type DefaultControllerType => typeof (PlayerController);

        public int Bullets = 6;
        public int MaxBullets = 6;

        public DiceRoll BulletDamageRoll = D4.Plus(2);

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

        public override void ResetAnimation() {
            if (HasStatusEffect<Overwatch>() || Enemies.Any(enemy => enemy.GetStatusEffect<LockedOn>()?.Caster == this)) {
                Play(Animations.Shoot);
            }
            else {
                Play(Animations.Idle);
            }
        }
    }
}