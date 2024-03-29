using System.Threading.Tasks;
using Utils;

namespace Combat {
    public class GhoulController : Controller {
        public Ghoul Ghoul => Combatant as Ghoul;

        public override async Task<CombatAction> RequestAction() {
            if (Ghoul.Tempo < 2) return null;

            if (Ghoul.Row == 0) {
                var targets = Ghoul.Actions.Punch.GetValidTargets();

                if (targets.Count > 0) return Ghoul.Actions.Punch.Bind(RNG.SelectFrom(targets).ToArray());
            }
            else {
                var targets = Ghoul.Actions.Charge.GetValidTargets();

                if (targets.Count > 0) return Ghoul.Actions.Charge.Bind(RNG.SelectFrom(targets).ToArray());
            }
            
            return null;
        }
    }
}