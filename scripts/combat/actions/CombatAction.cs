using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Combat {
    public abstract class CombatAction {
        public abstract string Name { get; }

        public virtual bool IsAvailable () {
            return true;
        }

        public Combatant User { get; protected set; }

        public CombatAction (Combatant user) {
            User = user;
        }

        public abstract List<TargetSelector> TargetSelectors { get; protected set; }
        public List<CombatTarget> Targets;

        public bool Bound { get; protected set; } = false;

        public CombatAction Bind (params CombatTarget [] targets) {
            Bound = true;
            Targets = new List<CombatTarget> (targets);
            return this;
        }

        public void Unbind () {
            Bound = false;
            Targets = null;
            Condition = () => true;
        }

        public Func<bool> Condition = () => true;
        public CombatAction WithCondition (Func<bool> condition) {
            Condition = condition;
            return this;
        }

        public abstract Task Run ();

        public async Task RequestBindAndRun () {
            var all_targets = Positioner.GetCombatTargets();
            var selected_targets = new List<CombatTarget> ();

            foreach (var selector in TargetSelectors) {
                var predicates = new List<Predicate<CombatTarget>> ();

                if (selector.Type != TargetType.Position) predicates.Add(target => target.Combatant != null);
                if (selector.Side != null) predicates.Add(target => (int) User.Side * (int) selector.Side == (int) target.Side);
                if (selector.Row != null) predicates.Add(target => target.Row == selector.Row);
                if (selector.Validator != null) predicates.Add(target => selector.Validator(target, User, selected_targets));

                var selectable_targets = new List<CombatTarget> ();
                foreach (var target in all_targets) {
                    if (predicates.Find(predicate => !predicate(target)) == null) selectable_targets.Add(target);
                }

                CombatTarget selection = null;
                switch (selector.Type) {
                    case TargetType.Position:
                        selection = await TargetingInterface.SelectPosition(selectable_targets);
                        break;
                    case TargetType.Single:
                        selection = await TargetingInterface.SelectSingleCombatant(selectable_targets);
                        break;
                }

                if (selection == null) {
                    return;
                }
                else {
                    selected_targets.Add(selection);
                }
            }

            await InteractionManager.Act(Bind(selected_targets.ToArray()));
        }
    }
}