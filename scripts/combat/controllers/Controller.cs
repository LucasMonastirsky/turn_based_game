using System;
using System.Threading.Tasks;
using Godot;

namespace Combat {
    public abstract partial class Controller : Node {
        public abstract void OnTurnStart ();

        public virtual Task<ICombatant> RequestSingleTarget (ICombatant user, TargetSelector selector) {
            throw new NotImplementedException("RequestSingleTarget not implemented in controller");
        }

        public virtual Task<CombatPosition> RequestPosition () {
            throw new NotImplementedException("RequestPosition not implemented in controller");
        }
    }
}