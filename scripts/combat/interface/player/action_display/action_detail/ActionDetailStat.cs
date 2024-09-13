using Godot;

public partial class ActionDetailStat : HBoxContainer {
	[Export] public TextureRect Icon;

	[Export] public Texture2D Texture {
		get => Icon.Texture;
		set { Icon.Texture = value; }
	}

	[Export] public Label Label;

	int _value;
	public int Value {
		get => _value;
		set {
			_value = value;
			Label.Text = $"{value}";
		}
	}
}
