using System.Threading;
using System.Threading.Tasks;
using Combat;
using CustomDebug;

public partial class Hugo {
    public class ActionStore {
        public class Swing : SingleTargetAction {
            public override string Name { get => "Swing"; }

            protected new Hugo user { get => base.user as Hugo; }
            public Swing (ICombatant user) : base(user) {}

            public override async Task Run (ICombatant target) {
                await user.Play(user.Animations.Swing).Frames[1].Task;

                var attack_result = ActionHelpers.BasicAttack(user, target, new ActionHelpers.BasicAttackOptions {
                    ParryNegation = -10, DodgeNegation = 10,
                });

                if (attack_result.Hit) {
                    target.Damage(10, new string[] { "Cut" });
                }

                await InteractionManager.ResolveQueue();

                user.Play(user.Animations.Idle);
                InteractionManager.EndAction();
            }

        }
    }
}