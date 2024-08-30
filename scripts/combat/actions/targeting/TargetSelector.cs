using System.Collections.Generic;

namespace Combat {
    public enum TargetType {
        Position,
        Single,
        Double,
    }

    public enum SideSelector {
        Same = 1,
        Opposite = -1,
    }

    public struct Selector {
        public TargetType Type { get; init; }
 
        public SideSelector? Side { get; init; } = null;
        public int? Row { get; init; } = null;

        public int? VerticalRange { get; init; } = null;

        public bool CanTargetSelf { get; init; } = false;
        public bool IsValidMovement { get; init; } = false;

        public delegate bool ValidatorDelegate (Target target, Combatant user, List<Target> previous_targets);
        public ValidatorDelegate Validator { get; init; } = null;

        public Selector (TargetType type) {
            Type = type;
        }        
    }
}