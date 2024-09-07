using Combat;
using Godot;
using System;
using System.Collections.Generic;

public partial class CombatantStatContainer : GridContainer {
	[Export] CombatantDetail CombatantDetail;

	[Export] StatLabel Damage, Armor, Hit, Crit, Parry, Dodge;

	List<StatLabel> Labels => new () { Damage, Armor, Hit, Crit, Parry, Dodge, };
	Combatant Combatant => CombatantDetail.Combatant;

	public override void _Process (double delta) {
		if (Combatant != null) {
			Damage.LabelValue.Text = $"{Combatant.DamageBonus}";
			Armor.LabelValue.Text = $"{Combatant.Armor}";
			Hit.LabelValue.Text = $"{Combatant.HitBonus}";
			Crit.LabelValue.Text = $"{Combatant.CritBonus}";
			Parry.LabelValue.Text = $"{Combatant.ParryBonus}";
			Dodge.LabelValue.Text = $"{Combatant.DodgeBonus}";
		}
	}
}
