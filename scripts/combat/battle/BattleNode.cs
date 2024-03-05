using System.Collections.Generic;
using Godot;

namespace Combat {
    public partial class BattleNode : Node {
        public List<Combatant> Combatants { get; protected set; }
    }
}