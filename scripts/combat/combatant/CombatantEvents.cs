namespace Combat {
    public partial class Combatant {
        public CombatantEvents Events = new ();

        public class CombatantEvents {
            public EventManager<Attack> BeforeAttack = new ();

            public EventManager<Movement> AfterMovement = new ();
        }
    }
}