using System.Collections.Generic;
using System.Linq;
using Combat;
using Development;
using Godot;
using ResourceHelpers;

public partial class CombatantDisplay : Control {
	[Export] Label MainLabel;
	[Export] Container EffectIconContainer;
	[Export] PackedScene EffectIconScene;

	private Dictionary<StatusEffect, TextureRect> Icons = new ();

	public Combatant User;

	public void AddStatusEffect (StatusEffect effect) {
		var icon = EffectIconScene.Instantiate<CombatantDisplayEffectIcon> ();
		icon.Effect = effect;
		EffectIconContainer.AddChild(icon);
		Icons[effect] = icon;
	}

	public void RemoveStatusEffect (StatusEffect effect) {
		Icons[effect].QueueFree();
		Icons.Remove(effect);
	}

	public override void _Process (double delta) {
		var health = User.ExtraHealth > 0 ? $"{User.Health}+{User.ExtraHealth}" : $"{User.Health}";
		MainLabel.Text = $"{User.Name} {health}/{User.MaxHealth} ({User.Tempo}T)";

		var position = Positioner.GetWorldPosition(User.Position);
		Position = position with { Y = position.Y - User.Node.Animator.Height * User.Node.Scale.Y - 20 };
	}

}
