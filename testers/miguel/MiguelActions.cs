using System.Collections.Generic;
using System.Threading.Tasks;
using Combat;

public partial class Miguel {
    public override List<CombatAction> ActionList => new (new CombatAction[] {
        Actions.Swing,
    });

    public ActionStore Actions;

    public class ActionStore {
        public ActionClasses.Swing Swing;

        public ActionStore (Miguel miguel) {
            Swing = new (miguel);
        }
    }

    public class ActionClasses {
        public class Swing : SingleTargetAction {
            public override string Name => "Swing";

            public override TargetSelector Selector => new TargetSelector() {
                Side = TargetSelector.SideCondition.Opposite,
                Row = 0,
            };

            protected new Miguel user => base.user as Miguel;
            public Swing (Miguel user) : base (user) {}

            public override async Task Run(ICombatant target) {
                user.Animator.Play(user.Animations.Swing);
                await user.MoveToMelee(target);

                var attack_result = ActionHelpers.BasicAttack(user, target, new ActionHelpers.BasicAttackOptions {
                    ParryNegation = 0, DodgeNegation = 0,
                });

                if (attack_result.Hit) {
                    target.Damage(8, new string[] { "Cut" });
                }

                await InteractionManager.ResolveQueue();

                await user.MoveTo(Positioner.GetWorldPosition(user.CombatPosition));

                user.Animator.Play(user.Animations.Idle);
                InteractionManager.EndAction();
            }
        }
    }
}