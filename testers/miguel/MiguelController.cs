using Combat;
using Utils;

public partial class MiguelController : Controller {
    public Combatant User;
    private Miguel miguel => User as Miguel;

    public override void OnTurnStart() {
        if (User.Row != 0) {
            if (RNG.Bool()) {
                miguel.Actions.Pass.Run();
            }
            else {
                var targets = Battle.Combatants.FindAll(combatant => !combatant.IsDead && combatant.Side == User.Side && combatant.Row == 0);
                miguel.Actions.Move.Run(RNG.SelectFrom(targets).CombatPosition);
            }
        }
        else {
            var targets = Battle.Combatants.FindAll(combatant => !combatant.IsDead && combatant.Side != User.Side && combatant.Row == 0);

            if (targets.Count > 0) _ = miguel.Actions.Swing.Run(RNG.SelectFrom(targets));
            else miguel.Actions.Pass.Run();
        }
    }
}