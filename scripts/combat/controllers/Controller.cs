using System;
using System.Threading.Tasks;
using Godot;

namespace Combat {
    public abstract partial class Controller : Node {
        public ICombatant Combatant;

        public abstract void OnTurnStart ();
        public virtual void OnTurnEnd () {}

        public virtual Task<ICombatant> RequestSingleTarget (ICombatant user, TargetSelector selector) {
            throw new NotImplementedException("RequestSingleTarget not implemented in controller");
        }

        public virtual Task<CombatPosition> RequestPosition (ICombatant user) {
            throw new NotImplementedException("RequestPosition not implemented in controller");
        }
    }
}