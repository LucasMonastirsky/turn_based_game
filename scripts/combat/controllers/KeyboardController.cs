using Combat;
using CustomDebug;
using Godot;
using System;
using System.Threading.Tasks;

public partial class KeyboardController : Node, IController {
    public ICombatant hugo, sasuke;

    public override void _Ready () {
        hugo = GetNode<StandardCombatant>("../Hugo");
        sasuke = GetNode<StandardCombatant>("../Sasuke");
    }

    public override void _Process(double delta) {
        if (Input.IsActionJustPressed("Test1")) {
            Dev.Log(Dev.TAG.INPUT, "Pressed Test1");
            new Hugo.ActionStore.Swing(hugo).RequestTargetsAndRun();
        }
    }

    public async Task<ICombatant> RequestSingleTarget (TargetSelector selector) {
        await Task.Delay(1);
        return sasuke;
    }
}
