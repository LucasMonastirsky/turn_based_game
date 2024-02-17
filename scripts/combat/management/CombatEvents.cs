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
    }
}