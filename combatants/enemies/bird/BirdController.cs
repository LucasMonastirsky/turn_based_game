using System.Threading.Tasks;
using Utils;

namespace Combat {
    public class BirdController : Controller {
        public Bird Bird => Combatant as Bird;

        public override async Task<CombatAction> RequestAction() {
            if (Bird.Tempo < 2) return null;

            var targets = Bird.Actions.Peck.GetValidTargets();

            if (targets.Count > 0) return Bird.Actions.Peck.Bind(RNG.SelectFrom(targets)[0]);
            
            return null;
        }
    }
}