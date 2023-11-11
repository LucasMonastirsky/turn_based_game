using Combat;
using Godot;

public partial class Hugo : StandardCombatant {
	[Export] public PackedScene PackedController;

	public override string CombatName { get => "Hugo"; }

	protected override void Setup () {
		Actions = new ActionStore(this);
		var scene = PackedController.Instantiate();
		AddChild(scene);
		Controller = scene as KeyboardController;
		Controller.Combatant = this;
	}
}
