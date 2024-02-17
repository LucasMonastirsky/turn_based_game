using System;
using System.Threading.Tasks;

namespace Combat {
    public abstract class Controller {
        public Combatant Combatant;

        public abstract Task OnTurnStart ();
        public virtual void OnTurnEnd () {}

        public virtual Task<CombatTarget> RequestSingleTarget (Combatant user, TargetSelector selector) {
            throw new NotImplementedException("RequestSingleTarget not implemented in controller");
        }

        public virtual Task<CombatPosition?> RequestPosition (Combatant user) {
            throw new NotImplementedException("RequestPosition not implemented in controller");
        }
    }
}