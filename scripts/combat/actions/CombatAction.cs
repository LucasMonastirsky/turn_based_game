using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Development;
using Utils;

namespace Combat {
    public abstract class CombatAction : Source {
        private int _id { get; } = RNG.NewId;
        public int Id => _id;
        public abstract string Name { get; }

        public abstract int TempoCost { get; set; }

        public virtual List<Restrictor> Restrictors { get; init; } = new () {};

        public virtual bool IsAvailable => User.Tempo >= TempoCost && !Restrictors.Any(restrictor => !restrictor.IsValid(this));

        public Combatant User { get; protected set; }

        public CombatAction (Combatant user) {
            User = user;
        }

        public virtual List<TargetSelector> TargetSelectors { get; protected set; } = new () {};
        public List<CombatTarget> Targets;
        public int TargetCount => TargetSelectors.Count;
        public CombatTarget Target => Targets[0];

        public bool Bound { get; protected set; } = false;

        public CombatAction Bind (params Targetable [] targetables) { // TODO: binding error checks
            Bound = true;
            Targets = targetables.Select(target => target.ToTarget()).ToList();;
            return this;
        }

        public CombatAction RandomBind (List<List<CombatTarget>> target_sets) {
            return Bind(RNG.SelectFrom(target_sets).ToArray());
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

        public async Task<CombatAction> Act () {
            if (!Bound) Dev.Error($"Tried to act unbound action {this}");

            await Run();

            return this;
        }

        public async Task Act (params Targetable [] targetables) {
            Bind(targetables);

            if (!PassesSelectors()) Dev.Error($"Action {this} does not pass selector");

            await Run();
            Unbind();
        }

        public async Task<CombatAction> RequestBind () {
            CombatPlayerInterface.HideActionList();

            var all_targets = Positioner.GetCombatTargets();
            Targets = new ();

            for (var i = 0; i < TargetSelectors.Count; i++) {
                var selector = TargetSelectors[i];
                var selectable_targets = new List<CombatTarget> ();

                foreach (var target in all_targets) {
                    if (IsValidTarget(target, selector)) selectable_targets.Add(target);
                }

                CombatTarget selection = null;
                switch (selector.Type) {
                    case TargetType.Position:
                        selection = await TargetingInterface.SelectPosition(selectable_targets);
                        break;
                    case TargetType.Single:
                        selection = await TargetingInterface.SelectSingleCombatant(selectable_targets);
                        break;
                    case TargetType.Double:
                        selection = await TargetingInterface.SelectPosition(selectable_targets);
                        break;
                }

                if (selection == null) {
                    Targets = new ();
                    CombatPlayerInterface.ShowActionList();
                    return null;
                }
                else {
                    Targets.Add(selection);
                }

                if (i != TargetSelectors.Count - 1) await Timing.Delay(1/5f);
            }

            return Bind(Targets.ToArray());
        }

        private bool IsValidTarget (CombatTarget target, TargetSelector selector, List<CombatTarget> previous_targets = null) {
            if (
                target.Combatant != null
                && target.Combatant.Side != User.Side
                && target.Combatant.HasStatusEffect<Hidden>()
            ) {
                return false;
            }

            if (previous_targets == null) previous_targets = Targets;

            var predicates = new List<Func<bool>> ();

            if (selector.Type == TargetType.Single) predicates.Add(() => target.Combatant != null);
            if (selector.Side != null) predicates.Add(() => User.Side.Value * (int) selector.Side == target.Side.Value);
            if (selector.Row != null) predicates.Add(() => target.Row == selector.Row);
            if (selector.VerticalRange != null) predicates.Add(() => Math.Abs(User.Slot - target.Slot) <= selector.VerticalRange);
            if (selector.Validator != null) predicates.Add(() => selector.Validator(target, User, previous_targets));
            if (!selector.CanTargetSelf) predicates.Add(() => target.Combatant != User);

            if (selector.Type == TargetType.Double) predicates.Add(() => {
                if (target.Slot is 0 or 4) return false;
                if (Positioner.GetSlotData(target.Position with { Slot = target.Slot - 1 }).Combatant == null) return false;
                if (Positioner.GetSlotData(target.Position with { Slot = target.Slot + 1 }).Combatant == null) return false;

                return true;
            });

            return !predicates.Any(predicate => !predicate());
        }

        public bool PassesSelectors () {
            for (var i = 0; i < TargetSelectors.Count; i++) {
                if (Targets?[i] is null || !IsValidTarget(Targets[i], TargetSelectors[i])) {
                    return false;
                }
            }

            return true;
        }

        public List<List<CombatTarget>> GetValidTargets () {
            return GetValidTargets(null, 0);
        }

        private List<List<CombatTarget>> GetValidTargets (List<List<CombatTarget>> previous, int selector_index = 0) {
            var results = new List<List<CombatTarget>> ();

            if (selector_index == 0) {
                Positioner.GetCombatTargets().ForEach(target => {
                    if (IsValidTarget(target, TargetSelectors[selector_index])) {
                        results.Add(new () { target });
                    }
                });
            }
            else {
                for (var i = 0; i < previous.Count; i++) {
                    Positioner.GetCombatTargets().ForEach(target => {
                        if (IsValidTarget(target, TargetSelectors[selector_index], previous[i])) {
                            var new_list = previous[i].ToList();
                            new_list.Add(target);
                            results.Add(new_list);
                        }
                    });
                }
            }

            if (results.Count == 0 || selector_index >= TargetSelectors.Count - 1) return results;
            else return GetValidTargets(results, selector_index + 1);
        }

        public override string ToString () {
            return Name;
            // return $"{User.Name}.{Name}";
        }
    }
}