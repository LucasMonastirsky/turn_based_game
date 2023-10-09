using System.Collections.Generic;
using CustomDebug;
using Godot;

namespace Combat {
	public static class Battle {
		public static IBattle Current { get; set; }
		public static List<ICombatant> Combatants => Current.Combatants;
		public static IPositioner Positioner => Current.Positioner;
	}
}
