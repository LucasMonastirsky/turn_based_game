using Combat;
using Godot;
using ResourceHelpers;

public partial class CombatantDisplayEffectIcon : TextureRect {
	[Export] public Label LevelLabel, NameLabel;

	public Effect Effect;

	private bool Hovered = false;

	public override void _Ready () {
		if (Effect.IconFilePath != null) {
			Texture = Resources.LoadTexture(Effect.IconFilePath);
		}

		NameLabel.Text = Effect.Name;
	}

	public override void _Process (double delta) {
		if (Effect.Level != 0) LevelLabel.Text = $"{Effect.Level}";
		else LevelLabel.Text = "";
	}

	public override void _Input (InputEvent @event) {
		if (@event is InputEventMouseMotion mouse_motion) {
			var (x, y) = GetScreenPosition();
			var (xw, yh) = (x + Size.X, y + Size.Y);
			var (mouse_x, mouse_y) = mouse_motion.GlobalPosition;

			Hovered = mouse_x > x && mouse_x < xw && mouse_y > y && mouse_y < yh;
			NameLabel.Visible = Hovered;
		}
	}
}
