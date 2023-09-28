using System.Threading.Tasks;
using CustomDebug;

namespace Combat {
    public delegate bool TargetSelector (ICombatant target);

    public abstract class Action {
        public abstract string Name { get; }

        protected ICombatant user;

        public Action (ICombatant user) {
            this.user = user;
        }

        public abstract Task RequestTargetsAndRun ();
    }

    public abstract class SingleTargetAction : Action {
        protected TargetSelector selector { get; }
        public SingleTargetAction (ICombatant user) : base (user) {}
        public abstract Task Run (ICombatant target);
        public override async Task RequestTargetsAndRun () {
            Dev.Log($"Requesting targets for {Name}");
            var target = await user.Controller.RequestSingleTarget(selector);
            await Run(target);
        }
    }

    // public abstract class GroupTargetAction

    public interface IActionStore {

    }
}