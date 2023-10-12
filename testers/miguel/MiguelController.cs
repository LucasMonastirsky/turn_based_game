using Combat;
using CustomDebug;

public partial class MiguelController : Controller {
    public ICombatant User;
    private Miguel miguel => User as Miguel;

    public override void OnTurnStart() {
        _ = new Miguel.ActionStore.Swing(miguel).Run(Battle.Combatants.Find(x => x.CombatName == "Hugo"));
    }
}