using System.Threading.Tasks;
using Combat;
using Utils;

public partial class MiguelController : Controller {
    private new Miguel Combatant => base.Combatant as Miguel;

    public override async Task<CombatAction> RequestAction () {
        if (Combatant.Row != 0) {
            var allies = Battle.Combatants.OnSide(Combatant.Side);

            if (allies.OnRow(0).Count < 3 && allies.OnRow(0).Count < allies.OnRow(1).Count) {
                var position = allies.OnRow(0).All[0].CombatPosition;
                var target = new CombatTarget (position with { Slot = position.Slot - 1 });
                return Combatant.Actions.Move.Bind(target);
            }
            else {
                var front_row_allies = Battle.Combatants.OnSide(Combatant.Side).OnRow(0).All;
                front_row_allies.Sort((a, b) => a.Health - b.Health);
                var most_injured = front_row_allies[0];

                if (most_injured.Health < Combatant.Health) {
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