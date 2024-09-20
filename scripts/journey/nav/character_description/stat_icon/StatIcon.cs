using Godot;
using System;

public partial class StatIcon : CenterContainer {
	[Export] public TextureRect TextureRect;
	[Export] public Texture2D Texture {
		get => TextureRect.Texture;
		set { TextureRect.Texture = value; }
	}
}
