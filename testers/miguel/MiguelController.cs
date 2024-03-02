using System.Linq;
using System.Threading.Tasks;
using Combat;
using Utils;

public partial class MiguelController : Controller {
    private new Miguel Combatant => base.Combatant as Miguel;

    public override async Task<CombatAction> RequestAction () {
        if (Combatant.Row == 1) {
            var front_allies = Combatant.Allies.OnRow(0);

            if (front_allies.Count < Combatant.Allies.OnRow(1).Count + 1) {
                var targets = Positioner.GetMoveTargets(Combatant).Where(target => target.Combatant is null).ToList();
                if (targets.Count > 0) return Combatant.Actions.Move.Bind(RNG.SelectFrom(targets));
            }

            var weak_front_allies = front_allies.Where(ally => ally.Health < Combatant.Health).ToList();
            if (weak_front_allies.Count > 0) {
                var allies_with_extra_tempo = weak_front_allies.Where(ally => ally.Tempo > 0).ToList();

                if (allies_with_extra_tempo.Count > 0) return Combatant.Actions.Move.Bind(RNG.SelectFrom(allies_with_extra_tempo));
                else if (Combatant.Tempo > 1) return Combatant.Actions.Switch.Bind(RNG.SelectFrom(weak_front_allies));
            }

            return Combatant.Actions.Pass.Bind();
        }
        else {
            var targets = Battle.Combatants.OnOppositeSide(Combatant.Side).OnRow(0).Alive.All;

            if (targets.Count < 1) return Combatant.Actions.Pass.Bind();
            else return Combatant.Actions.Swing.Bind(RNG.SelectFrom(targets));
        }
    }
}