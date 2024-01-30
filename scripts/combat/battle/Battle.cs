using System.Collections;
using System.Collections.Generic;

namespace Combat {
	public static class Battle {
		public static BattleInstance Current { get; set; }
		public static CombatantStore Combatants => new (Current.Combatants);
	}
}
