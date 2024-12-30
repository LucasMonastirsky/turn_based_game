using System;
using Development;
using Godot;

public partial class SlotButton : CenterContainer {
	public static int id_count = 0;
	public int id;

	public SlotButton () {
		id = id_count++;
	}

	[Export] TextureButton Button;
	[Export] TextureRect OverlayTexture;

	private SlotItem _item;
	public SlotItem Item {
		get => _item;
		set {
			_item = value;
			Button.TextureNormal = Item.IconTexture;
		}
	}

	public Texture2D Texture {
		set {
			Button.TextureNormal = value;
		}
	}

	private bool _selected = false;
	public bool Selected {
		get => _selected;
		set {
			_selected = value;
			OverlayTexture.Visible = value;
		}
	}

	public bool Hovered { get; private set; }
	public Action OnHovered = () => {};

	public Action OnPress {
		set {
			Button.Pressed += value;
		}
	}

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
