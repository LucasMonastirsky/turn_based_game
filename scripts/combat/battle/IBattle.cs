using System.Collections.Generic;

namespace Combat {
    public interface IBattle {
        IPositioner Positioner { get; }
        List<ICombatant> Combatants { get; }
    }
}