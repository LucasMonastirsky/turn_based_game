using System.Collections.Generic;

namespace Combat {
    public enum TargetType {
        Position,
        Single,
    }

    public enum SideSelector {
        Same = 1,
        Opposite = -1,
    }

    public struct TargetSelector {
        public TargetType Type { get; init; }
 
        public SideSelector? Side { get; init; } = null;
        public int? Row { get; init; } = null;

        public int? VerticalRange { get; init; } = null;

        public bool CanTargetSelf { get; init; } = false;

        public delegate bool ValidatorDelegate (CombatTarget target, Combatant user, List<CombatTarget> previous_targets);
        public ValidatorDelegate Validator { get; init; } = null;

        public TargetSelector (TargetType type) {
            Type = type;
        }

        
    }
}