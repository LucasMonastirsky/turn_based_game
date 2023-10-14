using System.Collections.Generic;
using System.Threading.Tasks;
using CustomDebug;

namespace Combat {
    public struct TargetSelector {
        public enum SideCondition {
            Same = 1,
            Opposite = -1,
        }
        public SideCondition? Side { get; init; }
        public int? Row { get; init; }

        public delegate bool ValidatorDelegate (ICombatant target);
        public ValidatorDelegate Validator { get; init; }

    }

    public abstract class CombatAction {
        public abstract string Name { get; }

        protected ICombatant user;

        public CombatAction (ICombatant user) {
            this.user = user;
        }

        public abstract Task RequestTargetsAndRun ();
    }

    public abstract class SingleTargetAction : CombatAction {
        public abstract TargetSelector Selector { get; }
        public SingleTargetAction (ICombatant user) : base (user) {}
        public abstract Task Run (ICombatant target);
        public override async Task RequestTargetsAndRun () {
            Dev.Log($"Requesting targets for {Name}");
            var target = await user.Controller.RequestSingleTarget(user, Selector);
            await Run(target);
        }
    }
}