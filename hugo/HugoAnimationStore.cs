using Godot;
using Combat;

public partial class HugoAnimationStore : StandardCombatant.AnimationStore {
    [Export] public override CombatAnimation Idle { get; set; }
    [Export] public override CombatAnimation Hurt { get; set; }
    [Export] public CombatAnimation Swing { get; set; }
}