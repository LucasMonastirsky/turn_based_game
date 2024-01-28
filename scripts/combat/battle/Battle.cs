using System.Collections.Generic;

namespace Combat {
	public static class Battle {
		public static IBattle Current { get; set; }
		public static List<Combatant> Combatants => Current.Combatants;
	}
}
