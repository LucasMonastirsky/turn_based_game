using System.Threading.Tasks;
using Utils;

namespace Combat {
    public class BirdController : Controller {
        public Bird Bird => Combatant as Bird;

        private bool has_screeched = false;

        public override async Task<CombatAction> RequestAction() {
            if (Bird.Tempo > 2 && !has_screeched) {
                has_screeched = true;
                return Bird.Actions.Screech.Bind();
            }

            if (!has_screeched && !Bird.HasStatusEffect<Evading>() && Bird.TotalHealth <= Bird.BaseMaxHealth / 2) {
                return Bird.Actions.Evade.Bind();
            }

            if (Bird.Tempo >= 2) {
                var targets = Bird.Actions.Peck.GetValidTargets();

                if (targets.Count > 0) return Bird.Actions.Peck.Bind(RNG.SelectFrom(targets)[0]);
            }
            
            return null;
        }
    }
}