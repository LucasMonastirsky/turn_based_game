using Combat;
using Godot;

public partial class SasukeAnimationStore : StandardCombatant.AnimationStore {
    [Export] public override CombatAnimation Idle { get; set; }
    [Export] public override CombatAnimation Hurt { get; set; } 
    [Export] public override CombatAnimation Parry { get; set; }
    [Export] public CombatAnimation Swing { get; set; }
}