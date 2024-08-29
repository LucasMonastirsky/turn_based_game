using System;

namespace Combat {
    public class Restrictor { // we will use this to put icons in the ui
        public virtual Predicate<CombatAction> IsValid { get; init; }

        public Restrictor () {}
        public Restrictor (Predicate<CombatAction> predicate) {
            IsValid = predicate;
        }
    }

    public static class CommonRestrictors {
        public static Restrictor FrontRow => new Restrictor () {
            IsValid = action => action.User.Row == 0,
        };

        public static Restrictor BackRow => new Restrictor () {
            IsValid = action => action.User.Row == 1,
        };
        
        public static Restrictor CanMove => new Restrictor () {
            IsValid = action => action.User.CanMove,
        };
    }
}