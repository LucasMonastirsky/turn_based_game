using System;

namespace Combat {
    public partial class Oda : Combatant {
        public override string Name => "Miguel";
        public override Type DefaultControllerType => typeof(PlayerController);

        protected override void Setup () {
            base.Setup();
            Actions = new ActionStore(this);

            Health = 20;
            MaxHealth = 20;

            HitBonus = 5;
            ParryBonus = 5;
            DodgeBonus = 3;

            AddStatusEffect(new Sheathed());

            Events.AfterMovement.Always(async movement => {
                if (Row == 1 && movement.Start.Row != movement.End.Row && !HasStatusEffect<Sheathed>()) AddStatusEffect(new Sheathed());
            });

            Play(Animations.SheathedIdle);
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