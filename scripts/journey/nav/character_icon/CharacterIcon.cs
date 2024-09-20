using Combat;
using Godot;

public partial class CharacterIcon : CenterContainer {
	[Export] TextureButton Button;

	public Texture2D Texture {
		get => Button.TextureNormal;
		set { Button.TextureNormal = value; }
	}

	private Combatant _combatant;
	public Combatant Combatant {
		get => _combatant;
		set {
			_combatant = value;
			Button.TextureNormal = value.Icon;
		}
	}

	public override void _Process (double delta) {
		if (Button.IsHovered()) {
			CharacterDescription.Combatant = Combatant;
		}
		else if (CharacterDescription.Combatant == Combatant) CharacterDescription.Combatant = null;
	}
}
