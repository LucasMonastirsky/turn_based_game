using Combat;
using Development;
using Utils;

public partial class MiguelController : Controller {
    private Miguel miguel => Combatant as Miguel;

    public override async void OnTurnStart() {
        if (miguel.Row != 0) {
            var front_row_allies = Battle.Combatants.OnSide(miguel.Side).OnRow(0).All;
            front_row_allies.Sort((a, b) => a.Health - b.Health);
            var most_injured = front_row_allies[0];

            if (most_injured.Health < miguel.Health) {
                await InteractionManager.RunAction(() => miguel.Actions.Move.Run(most_injured.CombatPosition));
            }
            else {
                await InteractionManager.RunAction(() => miguel.Actions.Pass.Run());
            }
        }
        else {
            var targets = Battle.Combatants.Alive.OnOppositeSide(miguel.Side).OnRow(0);

            if (targets.Count > 0) await InteractionManager.RunAction(() => miguel.Actions.Swing.Run(RNG.SelectFrom(targets)));
            else await InteractionManager.RunAction(() => miguel.Actions.Pass.Run());
        }
    }
}