using System;

namespace Combat {
    public partial class Oda : Combatant {
        public override string Name => "Miguel";
        public override Type DefaultControllerType => typeof(PlayerController);

        protected override void Setup () {
            base.Setup();
            Actions = new ActionStore(this);

            Health = 20;
            BaseMaxHealth = 20;

            BaseHitBonus = 5;
            BaseParryBonus = 5;
            BaseDodgeBonus = 3;

            AddStatusEffect(new Sheathed());

            Events.BeforeAttack.Always(async attack => {
                if (attack.Target.Combatant == this && !attack.IsMelee) AddRollModifier(new (this, RollTags.Parry) { Bonus = 10 });
            });

            Events.AfterMovement.Always(async movement => {
                if (Row == 1 && movement.Start.Row != movement.End.Row && !HasStatusEffect<Sheathed>()) AddStatusEffect(new Sheathed());
            });

            Play(Animations.SheathedIdle);
        }

        protected override void OnAttackParried(AttackResult attack_result) { // TODO: do this through events?
            base.OnAttackParried(attack_result);

            if (HasStatusEffect<Sheathed>()) RemoveStatusEffect<Sheathed>();
        }

        public override CombatAction GetRiposte (AttackResult attack_result) {
            if (!attack_result.Hit && attack_result.Attacker.Row == 0 && Row == 0) {
                return Actions.Swing.Bind(attack_result.Attacker);
            }

            return null;
        }

        public override void ResetAnimation() {
            if (IsDead) Animator.Play(StandardAnimations.Dead);
            else if (HasStatusEffect<Sheathed>()) Animator.Play(Animations.SheathedIdle);
            else Animator.Play(Animations.Idle);
        }
    }
}