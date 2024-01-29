using System;
using System.Threading.Tasks;
using Development;

namespace Combat {
    public struct TargetSelector {
        public enum SideCondition {
            Same = 1,
            Opposite = -1,
        }
        public SideCondition? Side { get; init; }
        public int? Row { get; init; }

        public Predicate<Combatant> Validator { get; init; }

    }

    public abstract class CombatAction {
        public abstract string Name { get; }

        public virtual bool IsAvailable () {
            return true;
        }

        protected Combatant user;

        public CombatAction (Combatant user) {
            this.user = user;
        }

        public abstract Task RequestTargetsAndRun ();
    }

    public abstract class SingleTargetAction : CombatAction {
        public abstract TargetSelector Selector { get; }
        public SingleTargetAction (Combatant user) : base (user) {}
        public abstract Task Run (Combatant target);
        public override async Task RequestTargetsAndRun () {
            // Dev.Log($"Requesting targets for {Name}");
            var target = await user.Controller.RequestSingleTarget(user, Selector);
            await Run(target);
        }
    }
}