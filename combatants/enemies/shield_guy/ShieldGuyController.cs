using System.Threading.Tasks;
using Utils;

namespace Combat {
    public class ShieldGuyController : Controller {
        public ShieldGuy ShieldGuy => Combatant as ShieldGuy;

        public override async Task<CombatAction> RequestAction() {
            if (ShieldGuy.Row == 1) {
                if (ShieldGuy.Tempo >= 2) {
                    return ShieldGuy.Actions.Bash.RandomBind();
                }
                if (ShieldGuy.Tempo == 1) {
                    return ShieldGuy.Actions.Move.RandomBind();
                }
            }
            else if (ShieldGuy.Tempo >= 2) {
                return ShieldGuy.Actions.Stab.RandomBind();
            }
            
            return ShieldGuy.Actions.Pass.Bind();
        }
    }
}