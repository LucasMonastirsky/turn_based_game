using System.Collections.Generic;
using Godot;

namespace Combat {
    public partial class BattleInstance : Node {
        public List<Combatant> Combatants { get; protected set; }
    }
}