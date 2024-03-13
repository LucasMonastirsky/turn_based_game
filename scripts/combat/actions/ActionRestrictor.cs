using System;

namespace Combat {
    public class ActionRestrictor {
        public virtual Predicate<CombatAction> IsValid { get; init; }
    }

    public static class ActionRestrictors {
        public static ActionRestrictor FrontRow => new ActionRestrictor () {
            IsValid = action => action.User.Row == 0,
        };

        public static ActionRestrictor BackRow => new ActionRestrictor () {
            IsValid = action => action.User.Row == 1,
        };
        
        public static ActionRestrictor CanMove => new ActionRestrictor () {
            IsValid = action => action.User.CanMove,
        };
    }
}