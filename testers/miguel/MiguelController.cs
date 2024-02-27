using System.Linq;
using System.Threading.Tasks;
using Combat;
using Utils;

public partial class MiguelController : Controller {
    private new Miguel Combatant => base.Combatant as Miguel;

    public override async Task<CombatAction> RequestAction () {
        if (Combatant.Tempo == 0) return Combatant.Actions.Pass.Bind();

        if (Combatant.Row == 1) {
            if (Combatant.Allies.OnRow(0).Count < Combatant.Allies.OnRow(1).Count + 1) {
                var targets = Positioner.GetMoveTargets(Combatant).Where(target => target.Combatant is null).ToList();
                return Combatant.Actions.Move.Bind(RNG.SelectFrom(targets));
            }
        }

        if (Combatant.Tempo == 1) {
            var available_allies = Combatant.Allies.OnRow(0).Where(ally => ally.Tempo > 0 && ally.Health < Combatant.Health);

            if (available_allies.Count() > 0) return Combatant.Actions.Move.Bind(available_allies.MinBy(ally => ally.Health));
            else return Combatant.Actions.Pass.Bind();
        }

        if (Combatant.Row == 0) {
            var targets = Battle.Combatants.OnOppositeSide(Combatant.Side).OnRow(0).Alive.All;

            if (targets.Count > 0) return Combatant.Actions.Swing.Bind(RNG.SelectFrom(targets));
            else return Combatant.Actions.Pass.Bind();
        }
        else {
            return Combatant.Actions.Pass.Bind();
        }
    }
}