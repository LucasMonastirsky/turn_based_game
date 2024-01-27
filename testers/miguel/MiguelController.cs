using System;
using Combat;
using CustomDebug;
using Godot;

public partial class MiguelController : Controller {
    public ICombatant User;
    private Miguel miguel => User as Miguel;

    public override void OnTurnStart() {
        if (User.Row != 0) {
            miguel.Actions.Pass.Run();
        }
        else {
            var targets = Battle.Combatants.FindAll(combatant => combatant.Side != User.Side && combatant.Row == 0);

            if (targets.Count > 0) _ = miguel.Actions.Swing.Run(targets[(int) GD.Randi() % (targets.Count - 1)]);
            else miguel.Actions.Pass.Run();
        }
    }
}