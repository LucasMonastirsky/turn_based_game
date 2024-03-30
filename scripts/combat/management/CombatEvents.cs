namespace Combat {
    public static class CombatEvents {
        public static EventManager<Combatant.Attack> BeforeAttack = new ();

        #region AfterAttack
        public struct AfterAttackArguments {
            public Combatant Attacker;
            public CombatTarget Target;
            public Combatant.Attack Options;
            public AttackResult Result;
        }
        public static EventManager<AfterAttackArguments> AfterAttack = new ();
        #endregion

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

        #region AfterMovement
        #endregion

        public static EventManager<CombatAction> BeforeAction = new ();
        public static EventManager<CombatAction> AfterAction = new ();
    }
}