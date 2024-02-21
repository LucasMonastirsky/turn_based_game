using System.Linq;
using System.Threading.Tasks;
using Combat;
using Utils;

public partial class MiguelController : Controller {
    private new Miguel Combatant => base.Combatant as Miguel;

    public override async Task<CombatAction> RequestAction () {
        if (Combatant.Tempo == 0) return Combatant.Actions.Pass.Bind();

        if (Combatant.Tempo == 1) {
            if (Combatant.Row == 1) {
                var available_allies = Combatant.Allies.OnRow(0).Where(ally => ally.Tempo > 0 && ally.Health < Combatant.Health);

                if (available_allies.Count() > 0) return Combatant.Actions.Move.Bind(available_allies.MinBy(ally => ally.Health));
            }

            return Combatant.Actions.Pass.Bind();
        }

        if (Combatant.Row != 0) {
            if (Combatant.Allies.OnRow(1).Any(ally => ally.IsDead)) return Combatant.Actions.Switch.Bind(Combatant.Allies.OnRow(1).First(ally => ally.IsDead));

            if (Combatant.Allies.OnRow(0).Count < 3 && Combatant.Allies.OnRow(0).Count < Combatant.Allies.OnRow(1).Count) {
                var position = Combatant.Allies.OnRow(0).All[0].CombatPosition;
                var target = new CombatTarget (position with { Slot = position.Slot - 1 });
                return Combatant.Actions.Move.Bind(target);
            }
            else {
                var front_row_allies = Battle.Combatants.OnSide(Combatant.Side).OnRow(0).All;
                front_row_allies.Sort((a, b) => a.Health - b.Health);
                var most_injured = front_row_allies[0];

                if (most_injured.Health < Combatant.Health && most_injured.Tempo > 0) {
                    return Combatant.Actions.Move.Bind(new CombatTarget(most_injured));
                }
                else {
                    return Combatant.Actions.Pass.Bind();
                }
            }
        }
        else {
            var targets = Battle.Combatants.Alive.OnOppositeSide(Combatant.Side).OnRow(0);

            if (targets.Count > 0) return Combatant.Actions.Swing.Bind(new CombatTarget(RNG.SelectFrom(targets)));
            else return Combatant.Actions.Pass.Bind();
        }
    }
}