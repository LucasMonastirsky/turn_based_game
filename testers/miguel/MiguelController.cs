using Combat;
using Utils;

public partial class MiguelController : Controller {
    private new Miguel Combatant => base.Combatant as Miguel;

    public override async void OnTurnStart() {
        if (Combatant.Row != 0) {
            var front_row_allies = Battle.Combatants.OnSide(Combatant.Side).OnRow(0).All;
            front_row_allies.Sort((a, b) => a.Health - b.Health);
            var most_injured = front_row_allies[0];

            if (most_injured.Health < Combatant.Health) {
                await InteractionManager.Act(Combatant.Actions.Move.Bind(most_injured.CombatPosition));
            }
            else {
                await InteractionManager.Act(Combatant.Actions.Pass.Bind());
            }
        }
        else {
            var targets = Battle.Combatants.Alive.OnOppositeSide(Combatant.Side).OnRow(0);

            if (targets.Count > 0) await InteractionManager.Act(Combatant.Actions.Swing.Bind(RNG.SelectFrom(targets)));
            else await InteractionManager.Act(Combatant.Actions.Pass.Bind());
        }
    }
}