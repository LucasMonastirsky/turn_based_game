using System.Collections.Generic;
using Combat;
using Godot;

public partial class CombatantDisplay : Node2D {
    public Combatant User;

    private List<Label> labels = new ();

    private Label LabelHealth, LabelTempo;
    private List<Label> EffectLabels = new ();

    public void AddStatusEffect (StatusEffect effect) {
        var label = new Label {
            Text = effect.Name,
            Position = LabelHealth.Position with { Y = LabelHealth.Position.Y + 10 * (EffectLabels.Count + 1), },
            Scale = new Vector2 { X = 0.4f, Y = 0.4f },
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
        var health_label = new Label {
            Scale = new Vector2 { X = 0.5f, Y = 0.5f },
        };

        AddChild(health_label);
        LabelHealth = health_label;
        labels.Add(health_label);
    }

    public override void _Process (double delta) {
        LabelHealth.Text = $"{User.Name} {User.Health}/{User.MaxHealth} ({User.Tempo}T)";

        var position = Positioner.GetWorldPosition(User.CombatPosition);
        Position = position with { Y = position.Y - 40, X = position.X - 40 };
    }

}
