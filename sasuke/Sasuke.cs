using Combat;
using Godot;
using System;

public partial class Sasuke : StandardCombatant {
    public override string CombatName { get => "Sasuke"; }

    protected override AnimationStore StandardAnimations { get => Animations; }
    [Export] protected SasukeAnimationStore Animations { get; set; }
}
