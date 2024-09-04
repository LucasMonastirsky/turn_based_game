using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace Combat {
    public class BoomerController : Controller {
        public Boomer Boomer => Combatant as Boomer;

        public override async Task<CombatAction> RequestAction() {
            if (Boomer.Tempo < 1) return null;

            if (Boomer.Row == 0) {
                if (Boomer.Tempo < 2) return null;

                var targets = Boomer.Actions.Spew.GetValidTargets();

                if (targets.Count > 0) return Boomer.Actions.Spew.Bind(RNG.SelectFrom(targets).ToArray());
            }

            if (Boomer.Row == 1) {
                var movement_targets = Boomer.Actions.Move.GetValidTargets().Where(set => set.All(target => target.Row == 0)).ToList();

                if (movement_targets.Count > 0) return Boomer.Actions.Move.RandomBind(movement_targets);
                else if (Boomer.Tempo > 1) return Boomer.Actions.BuildUp.Bind();
            }

            return null;
        }
    }
}