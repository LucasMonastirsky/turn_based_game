using System.Threading.Tasks;

namespace Combat {
    public abstract class Controller {
        public Combatant Combatant;

        public abstract Task<CombatAction> RequestAction ();
        public virtual void DeliverAction (CombatAction action) {}
        public virtual void CancelSelection () {}
    }
}