using System;
using System.Threading.Tasks;

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

        public Combatant User { get; protected set; }

        public CombatAction (Combatant user) {
            User = user;
        }

        public bool Bound { get; protected set; } = false;

        public void Unbind () {
            Bound = false;
            Condition = () => true;
        }

        public Func<bool> Condition = () => true;
        public CombatAction WithCondition (Func<bool> condition) {
            Condition = condition;
            return this;
        }

        public abstract Task RequestTargetsAndRun ();
        public abstract Task Run ();
    }

    public abstract class SingleTargetAction : CombatAction {
        public abstract TargetSelector Selector { get; }
        public SingleTargetAction (Combatant user) : base (user) {}

        public Combatant Target;
        public SingleTargetAction Bind (Combatant target) {
            Target = target;
            Bound = true;
            return this;
        }

        public override async Task RequestTargetsAndRun () {
            CombatPlayerInterface.HideActionList();

            var target = await User.Controller.RequestSingleTarget(User, Selector);

            if (target == null) {
                CombatPlayerInterface.ShowActionList();
                return;
            }

            await InteractionManager.Act(Bind(target));
        }
    }
}