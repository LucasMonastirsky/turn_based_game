namespace Combat {
	public static class Battle {
		public static BattleInstance Current { get; set; }
		public static CombatantStore Combatants => new (Current.Combatants);
	}
}
