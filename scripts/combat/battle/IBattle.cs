using System.Collections.Generic;

namespace Combat {
    public interface IBattle {
        List<ICombatant> Combatants { get; }
    }
}