using System;
using System.Collections.Generic;
using System.Linq;

namespace Combat {
    public abstract partial class CombatAction {
        private bool IsValidTarget (Target target, Selector selector, List<Target> previous_targets = null) {
            if (target.Combatant != null && target.Combatant.Side != User.Side && target.Combatant.HasStatusEffect<Hidden>()) {
                return false;
            }

            if (previous_targets == null) previous_targets = Targets;

            var predicates = new List<Func<bool>> ();

            if (target.Combatant != null && !target.Combatant.IsTargetableBy(this)) return false;

            if (selector.Type == TargetType.Single) predicates.Add(() => target.Combatant != null);
            if (selector.Side != null) predicates.Add(() => User.Side.Value * (int) selector.Side == target.Side.Value);
            if (selector.Row != null) predicates.Add(() => target.Row == selector.Row);
            if (selector.VerticalRange != null) predicates.Add(() => Math.Abs(User.Slot - target.Slot) <= selector.VerticalRange);
            if (selector.Validator != null) predicates.Add(() => selector.Validator(target, User, previous_targets));
            if (!selector.CanTargetSelf) predicates.Add(() => target.Combatant != User);
            if (selector.IsValidMovement) predicates.Add(() => Positioner.IsValidMovement(User, target.Position, false));

            if (selector.Type == TargetType.Double) predicates.Add(() => {
                if (target.Slot is 0 or 4) return false;

                var combatants = new List<Combatant> () {
                    Positioner.GetSlotData(target.Position with { Slot = target.Slot - 1 }).Combatant,
                    Positioner.GetSlotData(target.Position with { Slot = target.Slot + 1 }).Combatant
                };

                return combatants.All(combatant => combatant != null && combatant.IsTargetableBy(this));
            });

            return !predicates.Any(predicate => !predicate());
        }

        public bool PassesSelectors () {
            for (var i = 0; i < Selectors.Count; i++) {
                if (Targets?[i] is null || !IsValidTarget(Targets[i], Selectors[i])) {
                    return false;
                }
            }

            return true;
        }

        public List<List<Target>> GetValidTargets () {
            return GetValidTargets(null, 0);
        }

        private List<List<Target>> GetValidTargets (List<List<Target>> previous, int selector_index = 0) {
            var results = new List<List<Target>> ();

            if (selector_index == 0) {
                Positioner.GetCombatTargets().ForEach(target => {
                    if (IsValidTarget(target, Selectors[selector_index])) {
                        results.Add(new () { target });
                    }
                });
            }
            else {
                for (var i = 0; i < previous.Count; i++) {
                    Positioner.GetCombatTargets().ForEach(target => {
                        if (IsValidTarget(target, Selectors[selector_index], previous[i])) {
                            var new_list = previous[i].ToList();
                            new_list.Add(target);
                            results.Add(new_list);
                        }
                    });
                }
            }

            if (results.Count == 0 || selector_index >= Selectors.Count - 1) return results;
            else return GetValidTargets(results, selector_index + 1);
        }
    }
}