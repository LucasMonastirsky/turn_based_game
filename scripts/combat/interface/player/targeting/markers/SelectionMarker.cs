using Godot;

[Tool] public partial class SelectionMarker : Sprite2D {
	[Export] private int hitbox_width, hitbox_height, hitbox_low_padding;
	[Export] private bool is_debug_active;

	public bool EnableAutoHighlight = true;

	public delegate void OnCombatantSelectedDelegate ();
	public OnCombatantSelectedDelegate OnCombatantSelected;

	public EmptyDelegate OnHovered = () => {};

	public bool IsActive { get; private set; }

    public override void _Process (double delta) { // do this properly with events
		if (is_debug_active) {
			QueueRedraw();
		}

		if (EnableAutoHighlight) {
			var mouse_pos = GetLocalMousePosition();
			if (
				mouse_pos.X > -hitbox_width / 2
				&& mouse_pos.X < hitbox_width / 2
				&& mouse_pos.Y > -hitbox_height
				&& mouse_pos.Y < hitbox_low_padding
			) {
				IsActive = true;
				Scale = Scale with { X = 1.5f };
				OnHovered();
			}
			else {
				IsActive = false;
				Scale = Scale with { X = 1f };
			}

			if (IsActive && Input.IsMouseButtonPressed(MouseButton.Left)) { // TODO: create action for selection
				OnCombatantSelected();
			}
		}
    }

    public override void _Draw() {
        if (is_debug_active) DrawRect(
			new Rect2(
				new Vector2(-hitbox_width / 2, hitbox_low_padding),
				new Vector2(hitbox_width, -hitbox_low_padding - hitbox_height)
			),
			Colors.Green,
			false
		);
    }
}
