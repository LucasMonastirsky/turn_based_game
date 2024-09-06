using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace Combat {
    public class ShooterController : Controller {
        public Shooter Shooter => Combatant as Shooter;

        public override async Task<CombatAction> RequestAction() {
            if (Shooter.Row == 0) {
                if (Shooter.Tempo >= 2) {
                    return Shooter.Actions.Punch.RandomBind();
                }
                if (Shooter.Tempo == 1) {
                    var targets = Shooter.Allies.OnRow(1).Where(ally => ally.CanMove).ToList();

                    return Shooter.Actions.Move.RandomBind(targets);
                }
            }
            else if (Shooter.Tempo >= 2) {
                return Shooter.Actions.Shoot.RandomBind();
            }
            
            return Shooter.Actions.Pass.Bind();
        }
    }
}