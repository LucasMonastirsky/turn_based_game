using System;
using System.Collections.Generic;
using System.Linq;

namespace Combat {
    public partial class Combatant {
        public virtual bool IsTargetableBy (CombatAction action) {
            return TargetPredicates.Count == 0 || TargetPredicates.All(x => x(action));
        }

        public List<Predicate<CombatAction>> TargetPredicates = new () {};
    }
}