using System.Threading.Tasks;
using Combat;

public partial class Sasuke {
    public class ActionStore {
        public class Swing : SingleTargetAction {
            public override string Name { get => "Swing"; }

            protected new Sasuke user { get => base.user as Sasuke; }
            
            public Swing (ICombatant user) : base (user) {}

            public override async Task Run (ICombatant target) {
                await user.Play(user.Animations.Swing).Frames[0].Task;

                var attack_result = ActionHelpers.BasicAttack(user, target);

                if (attack_result.Hit) {
                    target.Damage(5, new string [] { "Cut" });
                }

                await InteractionManager.ResolveQueue();
                
                user.Play(user.Animations.Idle);
                InteractionManager.EndAction();
            }
        }
    }
}