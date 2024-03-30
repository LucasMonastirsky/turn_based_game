using System;
using System.Linq;
using Development;
using static Dice;

namespace Combat {
    public partial class Anna : Combatant {
        public override string Name => "Anna";

        public override Type DefaultControllerType => typeof (PlayerController);

        public int MaxBullets = 6;
        public int Bullets => GetStatusEffect<BulletsEffect>()?.Level ?? 0;

        private void SpendBullet () {
            if (Bullets < 1) Dev.Error("Tried to spend bullets without any");

            GetStatusEffect<BulletsEffect>().Level -= 1;
        }

        public DiceRoll BulletDamageRoll = D4.Plus(2);

        protected override void Setup () {
            base.Setup();
            Actions = new (this);

            Health = 20;
            MaxHealth = 20;

            HitBonus = 2;
            ParryBonus = 0;
            DodgeBonus = 2;

            AddStatusEffect(new BulletsEffect (MaxBullets));

            CombatEvents.BeforeAttack.Always(async (attack) => {
                if (attack.Attacker == this && attack.IsCrit) {
                    attack.DamageRoll.Times(2);
                }
            });
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

        public class BulletsEffect : StatusEffect {
            public override string Name => "Bullets";

            public BulletsEffect (int amount) {
                Level = amount;
            }
        }
    }
}