using Combat;
using Godot;
using System;

public partial class StatLabel : HBoxContainer {
	public Stat Stat;
	public Combatant Combatant;

	[Export] private Label LabelTitle, LabelValue;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		LabelTitle.Text = Stat.ToString();
		LabelValue.Text = $"{Combatant?.GetTotalBonuses(Stat) ?? -1}";
	}
}
