using System.Collections.Generic;

namespace Combat {
    public interface IBattle {
        List<Combatant> Combatants { get; }
    }
}