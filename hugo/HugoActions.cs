using System.Threading.Tasks;
using Combat;
using CustomDebug;

public partial class Hugo {
    public class ActionStore {
        public class Swing : SingleTargetAction {
            public override string Name { get => "Swing"; }

            public override TargetSelector Selector {
                get => new TargetSelector {
                    Side = TargetSelector.SideCondition.Opposite,
                    Row = 0,
                };
            }

            protected new Hugo user { get => base.user as Hugo; }
            public Swing (ICombatant user) : base(user) {}

            public override async Task Run (ICombatant target) {
                user.Animator.Play(user.Animations.Swing);

                var attack_result = ActionHelpers.BasicAttack(user, target, new ActionHelpers.BasicAttackOptions {
                    ParryNegation = 0, DodgeNegation = 0,
                });

                if (attack_result.Hit) {
                    target.Damage(10, new string[] { "Cut" });
                }

                await InteractionManager.ResolveQueue();

                user.Animator.Play(user.Animations.Idle);
                InteractionManager.EndAction();
            }

        }
    }
}