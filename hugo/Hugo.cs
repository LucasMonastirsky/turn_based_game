using CustomDebug;
using Godot;
using Combat;

public partial class Hugo : StandardCombatant {
	public override string CombatName { get => "Hugo"; }

	protected override AnimationStore StandardAnimations { get => Animations; }
	[Export] protected HugoAnimationStore Animations { get; set; }

	public override void _Ready () {
		base._Ready();
		Controller = GetNode<KeyboardController>("../KeyboardController");
	}
}
