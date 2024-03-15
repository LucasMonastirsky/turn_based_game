namespace Combat {
    public static class CombatEvents {
        #region BeforeAttack
        public struct BeforeAttackArguments {
            public Combatant Attacker;
            public CombatTarget Target;
            public Combatant.BasicAttackOptions Options; // replace this class with an attack class
        }

        public static EventManager<BeforeAttackArguments> BeforeAttack = new ();
        #endregion

        #region AfterAttack
        public struct AfterAttackArguments {
            public Combatant Attacker;
            public CombatTarget Target;
            public Combatant.BasicAttackOptions Options;
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