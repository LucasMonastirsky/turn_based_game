namespace Combat {
    public static class CombatEvents {
        #region BeforeAttack
        public struct BeforeAttackArguments {
            public Combatant Attacker;
            public CombatTarget Target;
            public Combatant.BasicAttackOptions Options;
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
    }
}