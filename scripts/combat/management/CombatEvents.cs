namespace Combat {
    public static class CombatEvents {
        public static EventManager<Combatant.Attack> BeforeAttack = new ();

        public static EventManager<AttackResult> AfterAttack = new ();

        #region BeforeTurnEnd
        public static EventManager BeforeTurnEnd = new ();
        #endregion
    
        #region AfterDeath
        public struct AfterDeathArguments {
            public Combatant Combatant;
        }
        public static EventManager<AfterDeathArguments> AfterDeath = new ();
        #endregion
    
        public static EventManager<Movement> BeforeMovement = new ();
        public static EventManager<Movement> AfterMovement = new ();

        #region AfterMovement
        #endregion

        public static EventManager<CombatAction> BeforeAction = new ();
        public static EventManager<CombatAction> AfterAction = new ();
    }
}