using System;
using System.Threading.Tasks;

namespace Combat {
    public abstract class Controller {
        public Combatant Combatant;

        public abstract Task<CombatAction> RequestAction ();
    }
}