using System.Collections.Generic;
using Combat;
using Development;
using Godot;

public partial class CombatantDisplay : Node2D {
    public Combatant User;

    private List<Label> labels = new ();

    private Label LabelHealth;

    public override void _Ready () {
        var label = new Label {
            Scale = new Vector2 { X = 0.65f, Y = 0.65f },
        };

        AddChild(label);
        LabelHealth = label;
        labels.Add(label);
    }

    public override void _Process (double delta) {
        LabelHealth.Text = $"HP: {User.Health}/{User.MaxHealth}";

        var position = Positioner.GetWorldPosition(User.CombatPosition);
        LabelHealth.Position = position with { Y = position.Y - 40, X = position.X - 20 };
    }

}
