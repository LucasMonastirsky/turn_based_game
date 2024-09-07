using Combat;
using Godot;
using System;

public partial class StatLabel : HBoxContainer {
	public Stat Stat;
	public Combatant Combatant;

	[Export] public Texture2D Texture {
		get => Icon.Texture;
		set { Icon.Texture = value; }
	}

	[Export] public TextureRect Icon;
	[Export] public Label LabelValue;
}
