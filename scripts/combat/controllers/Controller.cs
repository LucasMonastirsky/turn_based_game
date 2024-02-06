using System;
using System.Threading.Tasks;

namespace Combat {
    public abstract class Controller {
        public Combatant Combatant;

        public abstract void OnTurnStart ();
        public virtual void OnTurnEnd () {}

        public virtual Task<Combatant> RequestSingleTarget (Combatant user, TargetSelector selector) {
            throw new NotImplementedException("RequestSingleTarget not implemented in controller");
        }

        public virtual Task<CombatPosition?> RequestPosition (Combatant user) {
            throw new NotImplementedException("RequestPosition not implemented in controller");
        }
    }
}