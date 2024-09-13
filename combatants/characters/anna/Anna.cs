using System;
using System.Linq;
using Development;
using Godot;
using static Dice;

namespace Combat {
    public partial class Anna : Combatant {
        public override string Name => "Anna";

        public override Type DefaultControllerType => typeof (PlayerController);

        public int MaxBullets = 6;
        public int Bullets => GetStatusEffect<Loaded>()?.Level ?? 0;

        public bool IsLockedOn => Enemies.Any(enemy => enemy.GetStatusEffect<LockedOn>()?.Caster == this);
        public Combatant LockedOnTarget => Enemies.Where(enemy => enemy.GetStatusEffect<LockedOn>()?.Caster == this).FirstOrDefault();

        private void SpendBullet () {
            if (Bullets < 1) Dev.Error("Tried to spend bullets without any");

            GetStatusEffect<Loaded>().Level -= 1;
        }

        public int BulletDamage = 6;

        protected override void Setup () {
            base.Setup();
            Actions = new (this);

            BaseMaxHealth = 30;

            BaseHitBonus = 5;
            BaseParryBonus = 0;
            BaseDodgeBonus = 3;
            BaseCritBonus = 1;

            AddStatusEffect(new Loaded (MaxBullets));

            CombatEvents.BeforeAttack.Always(async (attack) => {
                if (attack.Attacker == this && attack.IsCrit) {
                    attack.DamageAmount = Mathf.RoundToInt(attack.DamageAmount * 1.5f);
                }
            });
        }

        public override CombatAction GetRiposte (AttackResult attack_result) {
            if (attack_result.Dodged) {
                if (attack_result.Attack.IsMelee) return Actions.Kick.Bind(attack_result.Attacker);
                if (Bullets > 0 && (!IsLockedOn || LockedOnTarget == attack_result.Attacker)) {
                    return Actions.Shoot.Bind(attack_result.Attacker);
                }
            }

            return null;
        }

        public override void OnTurnEnd () {
            AddStatusEffect(new TheShakes ());
        }

        public override void ResetAnimation() {
            if (HasStatusEffect<Overwatch>() || IsLockedOn) {
                Play(Animations.Shoot);
            }
            else {
                Play(Animations.Idle);
            }
        }

        public class Loaded : StatusEffect {
            public override string Name => "Loaded";
            public override string IconFilePath => "res://combatants/characters/anna/resources/icons/effects/icon_loaded.png";

            public Loaded (int amount) {
                Level = amount;
            }
        }
    }
}