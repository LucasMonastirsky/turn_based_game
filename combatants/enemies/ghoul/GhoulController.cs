using System.Threading.Tasks;
using Utils;

namespace Combat {
    public class GhoulController : Controller {
        public Ghoul Ghoul => Combatant as Ghoul;

        public override async Task<CombatAction> RequestAction() {
            if (Ghoul.Tempo < 2) return null;

            if (Ghoul.Row == 0) {
                return Ghoul.Actions.Punch.RandomBind();
            }
            else {
                return Ghoul.Actions.Charge.RandomBind();
            }
        }
    }
}