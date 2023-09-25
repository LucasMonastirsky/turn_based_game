using Combat;
using Godot;
using System;

public partial class TobiAnimations : StandardCombatant.AnimationStore {
    [Export] public override CombatAnimation Idle { get; set; }
    [Export] public override CombatAnimation Hurt { get; set; }

}
