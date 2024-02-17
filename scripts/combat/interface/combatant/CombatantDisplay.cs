using System.Collections.Generic;
using Combat;
using Development;
using Godot;

public partial class CombatantDisplay : Node2D {
    public Combatant User;

    private List<Label> labels = new ();

    private Label LabelHealth;
    private List<Label> EffectLabels = new ();

    public void AddStatusEffect (StatusEffect effect) {
        var label = new Label {
            Text = effect.Name,
            Position = LabelHealth.Position with { Y = LabelHealth.Position.Y + 10, },
            Scale = new Vector2 { X = 0.65f, Y = 0.65f },
        };
        AddChild(label);
        EffectLabels.Add(label);
    }

    public void RemoveStatusEffect (StatusEffect effect) {
        var label = EffectLabels.Find(label => label.Text == effect.Name);
        EffectLabels.Remove(label);
        label.QueueFree();
    }

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
