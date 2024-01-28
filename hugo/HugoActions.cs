using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Combat;

public partial class Hugo {
    public override List<CombatAction> ActionList => new (new CombatAction[] {
        Actions.Swing,
        Actions.Move,
        Actions.Pass,
    });

    public ActionStore Actions;

    public class ActionStore {
        public HugoActions.Swing Swing;

        public CommonActions.Move Move;
        public CommonActions.Pass Pass;

        public ActionStore (Hugo hugo) {
            Swing = new (hugo);
            Move = new (hugo);
            Pass = new (hugo);
        }
    }

    public abstract class HugoActions {
        public class Swing : SingleTargetAction {
            public override string Name { get => "Swing"; }

            public override bool IsAvailable () {
                return user.Row == 0;
            }

            public override TargetSelector Selector => new () {
                Side = TargetSelector.SideCondition.Opposite,
                Row = 0,
            };

            protected new Hugo user { get => base.user as Hugo; }
            public Swing (Combatant user) : base(user) {}

            public override async Task Run (Combatant target) {
                user.Animator.Play(user.Animations.Swing);
                await user.MoveToMelee(target);

                var attack_result = ActionHelpers.BasicAttack(user, target, new ActionHelpers.BasicAttackOptions {
                    ParryNegation = 0, DodgeNegation = 0,
                });

                if (attack_result.Hit) {
                    target.Damage(10, new string[] { "Cut" });
                }

                await InteractionManager.ResolveQueue();

                await user.MoveTo(Positioner.GetWorldPosition(user.CombatPosition));

                user.Animator.Play(user.Animations.Idle);
                InteractionManager.EndAction();
            }

        }
    }
}