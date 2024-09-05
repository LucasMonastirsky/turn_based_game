using System;

namespace Combat {
    public partial class Oda : Combatant {
        public override string Name => "Oda";
        public override Type DefaultControllerType => typeof(PlayerController);

        protected override void Setup () {
            base.Setup();
            Actions = new ActionStore(this);

            Health = 20;
            BaseMaxHealth = 20;

            BaseHitBonus = 5;
            BaseParryBonus = 15;
            BaseDodgeBonus = 3;

            Passives = new () {
                new Dojutsu (this),
                new Iaido (this),
            };
        }

        public override CombatAction GetRiposte (AttackResult attack_result) {
            if (attack_result.Attack.IsMelee && !attack_result.Hit) {
                return Actions.Swing.Bind(attack_result.Attacker);
            }

            if (!attack_result.Attack.IsMelee && attack_result.Parried) { // todo: make an action for this
                attack_result.Attacker.Damage(Roll(attack_result.Attack.DamageRoll, Stat.Damage), this);
            }

            return null;
        }

        public override void ResetAnimation() {
            if (IsDead) Animator.Play(Animations.Dead);
            else if (HasStatusEffect<Sheathed>()) Animator.Play(Animations.SheathedIdle);
            else Animator.Play(Animations.Idle);
        }
    }
}