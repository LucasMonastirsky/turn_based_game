using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Development;
using Utils;

namespace Combat {
    public abstract class CombatAction : Identifiable {
        private int _id { get; } = RNG.NewId;
        public int Id => _id;
        public abstract string Name { get; }

        public abstract int TempoCost { get; set; }

        public virtual bool IsAvailable () {
            return true;
        }

        public Combatant User { get; protected set; }

        public CombatAction (Combatant user) {
            User = user;
        }

        public virtual List<TargetSelector> TargetSelectors { get; protected set; } = new () {};
        public List<CombatTarget> Targets;

        public bool Bound { get; protected set; } = false;

        public CombatAction Bind (params Targetable [] targetables) { // TODO: binding error checks
            Bound = true;
            Targets = targetables.Select(target => target.ToTarget()).ToList();;
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

        public async Task Act () {
            if (!Bound) Dev.Error($"Tried to act unbound action {this}");

            await Run();
            Unbind();
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

            foreach (var selector in TargetSelectors) {
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
                }

                if (selection == null) {
                    Targets = new ();
                    CombatPlayerInterface.ShowActionList();
                    return null;
                }
                else {
                    Targets.Add(selection);
                }
            }

            return Bind(Targets.ToArray());
        }

        private bool IsValidTarget (CombatTarget target, TargetSelector selector) {
            var predicates = new List<Func<bool>> ();

            if (selector.Type != TargetType.Position) predicates.Add(() => target.Combatant != null);
            if (selector.Side != null) predicates.Add(() => (int) User.Side * (int) selector.Side == (int) target.Side);
            if (selector.Row != null) predicates.Add(() => target.Row == selector.Row);
            if (selector.VerticalRange != null) predicates.Add(() => Math.Abs(User.Slot - target.Slot) <= selector.VerticalRange);
            if (selector.Validator != null) predicates.Add(() => selector.Validator(target, User, Targets));
            if (!selector.CanTargetSelf) predicates.Add(() => target.Combatant != User);

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

        public override string ToString () {
            return Name;
            // return $"{User.Name}.{Name}";
        }
    }
}