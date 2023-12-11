using System.Collections.Generic;

namespace Combat {
	public static class Battle {
		public static IBattle Current { get; set; }
		public static List<ICombatant> Combatants => Current.Combatants;
	}
}
