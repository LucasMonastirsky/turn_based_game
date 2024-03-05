namespace Combat {
	public static class Battle {
		public static BattleNode Node { get; set; }
		public static CombatantStore Combatants => new (Node.Combatants);
	}
}
