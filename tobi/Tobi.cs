using Combat;
using Godot;
using System;

public partial class Tobi : StandardCombatant {
    public override string CombatName { get => "Tobi"; }
    protected override AnimationStore StandardAnimations { get => Animations; }
	[Export] protected TobiAnimations Animations { get; set; }
}
