using System.Collections.Generic;
using System.Linq;
using Combat;
using Godot;

public partial class CombatantDisplay : Node2D {
    public Combatant User;

    private List<Label> labels = new ();

    private Label LabelHealth, LabelTempo;
    private Dictionary<string, Label> EffectLabels = new ();

    public void AddStatusEffect (StatusEffect effect) {
        var label = new Label {
            Text = effect.Name,
            Position = LabelHealth.Position with { Y = LabelHealth.Position.Y + 10 * (EffectLabels.Count + 1), },
            Scale = new Vector2 { X = 0.75f, Y = 0.75f },
        };
        AddChild(label);
        EffectLabels.TryAdd(effect.Name, label);
    }

    public void RemoveStatusEffect (StatusEffect effect) {
        if (EffectLabels.ContainsKey(effect.Name)) {
            EffectLabels[effect.Name].QueueFree();
            EffectLabels.Remove(effect.Name);

            var effect_label_list = EffectLabels.Values.ToList();
            for (var i = 0; i < effect_label_list.Count(); i++) {
                effect_label_list[i].Position = LabelHealth.Position with { Y = LabelHealth.Position.Y + 10 * (i + 1) };
            }
        }
    }

    public override void _Ready () {
        var health_label = new Label {
            Position = new Vector2 { X = 0, Y = -75, },
        };

        AddChild(health_label);
        LabelHealth = health_label;
        labels.Add(health_label);
    }

    public override void _Process (double delta) {
        var health = User.ExtraHealth > 0 ? $"{User.Health}+{User.ExtraHealth}" : $"{User.Health}";
        LabelHealth.Text = $"{User.Name} {health}/{User.MaxHealth} ({User.Tempo}T) {User.HitBonus}";

        var position = Positioner.GetWorldPosition(User.Position);
        Position = position with { Y = position.Y - 75, X = position.X - 40 };

        foreach (var kvp in EffectLabels) {
            kvp.Value.Text = User.StatusEffects.Find(x => x.Name == kvp.Key)?.ToString();
        }
    }

}
