using System;
using System.Collections.Generic;
using System.Linq;
using Combat;
using Development;
using Godot;
using ResourceHelpers;

public partial class CombatantDisplay : Control {
	[Export] Label MainLabel;
	[Export] Container EffectIconContainer, TempoIconContainer;
	[Export] PackedScene EffectIconScene;
	[Export] Texture2D TempoIconTexture;

	private Dictionary<Effect, TextureRect> Icons = new ();
	private List<TextureRect> TempoIcons = new ();

	public Combatant User;

	public void AddStatusEffect (Effect effect) {
		var icon = EffectIconScene.Instantiate<CombatantDisplayEffectIcon> ();
		icon.Effect = effect;
		EffectIconContainer.AddChild(icon);
		Icons[effect] = icon;
	}

	public void RemoveStatusEffect (Effect effect) {
		Icons[effect].QueueFree();
		Icons.Remove(effect);
	}

	public override void _Process (double delta) {
		var health = User.ExtraHealth > 0 ? $"{User.Health}+{User.ExtraHealth}" : $"{User.Health}";
		MainLabel.Text = $"{User.Name} {health}/{User.MaxHealth} ({User.Tempo}T)";

		var position = Positioner.GetWorldPosition(User.Position);
		Position = position with { Y = position.Y - User.Node.Animator.Height * User.Node.Scale.Y - 20 };

		if (User.Tempo != TempoIcons.Count) {
			TempoIcons.ForEach(icon => icon.QueueFree());

			TempoIcons = new ();

			for (var i = 0; i < User.Tempo; i++) {
				var icon = new TextureRect {
					Texture = TempoIconTexture,
					ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
					Size = new (16, 16),
					Position = new (8 * i, 0),
				};

				TempoIconContainer.AddChild(icon);
				TempoIcons.Add(icon);
			}
		}
	}

}
