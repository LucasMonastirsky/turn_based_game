using Godot;
using System;

[Tool]
public partial class StatIcon : CenterContainer {
	[Export] public Label Label;
	[Export] public TextureRect TextureRect;
	[Export] public Texture2D Texture {
		get => TextureRect.Texture;
		set { 
			if (TextureRect != null) TextureRect.Texture = value;
		}
	}

	private int _value;
	public int Value {
		get => _value;
		set {
			_value = value;
			Label.Text = $"{value}";
		}
	}
}
