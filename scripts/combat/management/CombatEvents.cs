using System.Collections.Generic;

namespace Combat {
    public static class CombatEvents {
        public static EventManager<Attack> BeforeAttack = new ();
        public static EventManager<AttackResult> AfterAttack = new ();


        public static EventManager<Damage> BeforeDamage = new ();
        public static EventManager<Damage> AfterDamage = new ();

        public static EventManager<Combatant> BeforeTurnEnd = new ();
    
        public struct AfterDeathArguments {
            public Combatant Combatant;
        }
        public static EventManager<AfterDeathArguments> AfterDeath = new ();
    
        public static EventManager<Movement> BeforeMovement = new ();
        public static EventManager<Movement> AfterMovement = new ();


        public static EventManager<CombatAction> BeforeAction = new ();
        public static EventManager<CombatAction> AfterAction = new ();
    }
}