using System.Linq;
using System.Threading.Tasks;
using Combat;
using Development;
using Utils;

public partial class MiguelController : Controller {
    private new Miguel Combatant => base.Combatant as Miguel;

    public override async Task<CombatAction> RequestAction () {
        await Timing.Delay();

        if (Combatant.Row == 1) {
            Dev.Log(Dev.Tags.BotController, "Back row");

            var front_allies = Combatant.Allies.OnRow(0);

            if (Combatant.CanMove) {
                Dev.Log(Dev.Tags.BotController, "Can move");

                if (front_allies.Count < Combatant.Allies.OnRow(1).Count + 1) {
                    Dev.Log(Dev.Tags.BotController, $"Less front allies than back allies");

                    var targets = Positioner.GetMoveTargets(Combatant, false).Where(target => target.Combatant is null).ToList();
                    if (targets.Count > 0) {
                        Dev.Log(Dev.Tags.BotController, "Found valid targets");
                        return Combatant.Actions.Move.Bind(RNG.SelectFrom(targets));
                    }
                    else Dev.Log(Dev.Tags.BotController, "Found no valid targets");
                }

                Dev.Log(Dev.Tags.BotController, "Checking for weak allies");

                var weak_front_allies = front_allies.Where(ally => ally.Health < Combatant.Health).ToList();
                if (weak_front_allies.Count > 0) {
                    Dev.Log(Dev.Tags.BotController, $"Found weak allies");

                    var allies_with_extra_tempo = weak_front_allies.Where(ally => ally.Tempo > 0).ToList();

                    if (allies_with_extra_tempo.Count > 0) return Combatant.Actions.Move.Bind(RNG.SelectFrom(allies_with_extra_tempo));
                    else if (Combatant.Tempo > 1) return Combatant.Actions.Switch.Bind(RNG.SelectFrom(weak_front_allies));
                }
            }

            Dev.Log(Dev.Tags.BotController, "Checking for unimmobilized enemies");

            var unimmobilized_enemies = Combatant.Enemies.Where(enemy => !enemy.HasStatusEffect<Immobilized>()).ToList();
            if (unimmobilized_enemies.Count > 0) return Combatant.Actions.Immobilize.Bind(RNG.SelectFrom(Combatant.Enemies));
            else return null;
        }
        else {
            Dev.Log(Dev.Tags.BotController, "Front row");

            var targets = Battle.Combatants.OnOppositeSide(Combatant.Side).OnRow(0).Alive.All;

            if (Combatant.Tempo < 2 || targets.Count < 1) return null;
            else return Combatant.Actions.Swing.Bind(RNG.SelectFrom(targets));
        }
    }
}