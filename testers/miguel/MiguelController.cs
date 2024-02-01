using Combat;
using Development;
using Utils;

public partial class MiguelController : Controller {
    private Miguel miguel => Combatant as Miguel;

    public MiguelController () {
        Dev.Log("constructor");
    }

    public override void OnTurnStart() {Dev.Log("turnstart");
        if (miguel.Row != 0) {
            var front_row_allies = Battle.Combatants.OnSide(miguel.Side).OnRow(0).All;
            front_row_allies.Sort((a, b) => a.Health - b.Health);
            var most_injured = front_row_allies[0];

            if (most_injured.Health < miguel.Health) {
                miguel.Actions.Move.Run(most_injured.CombatPosition);
            }
            else {
                miguel.Actions.Pass.Run();
            }
        }
        else {
            var targets = Battle.Combatants.Alive.OnOppositeSide(miguel.Side).OnRow(0);

            if (targets.Count > 0) _ = miguel.Actions.Swing.Run(RNG.SelectFrom(targets));
            else miguel.Actions.Pass.Run();
        }
    }
}