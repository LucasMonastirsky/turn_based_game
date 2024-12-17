using System;
using Godot;

public partial class SlotButton : CenterContainer {
	[Export] TextureButton Button;
	[Export] TextureRect OverlayTexture;

	private bool _selected = false;
	public bool Selected {
		get => _selected;
		set {
			_selected = value;
			OverlayTexture.Visible = value;
		}
	}

	public bool Hovered { get; private set; }

	public Texture2D Texture {
		get => Button.TextureNormal;
		set { Button.TextureNormal = value; }
	}

	public Action OnPress {
		set {
			Button.Pressed += value;
		}
	}

	public Action OnHovered = () => {};

	public override void _Process (double delta) {
		if (!Hovered && Button.IsHovered()) {
			Hovered = true;
			OnHovered();
		}
		else if (Hovered && !Button.IsHovered()) {
			Hovered = false;
		}
	}
}
