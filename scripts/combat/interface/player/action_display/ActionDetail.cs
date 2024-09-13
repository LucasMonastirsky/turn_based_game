using System.Collections.Generic;
using Combat;
using Development;
using Godot;

public partial class ActionDetail : VBoxContainer {
	[Export] public Label Title;
	[Export] Control StatContainer;
	[Export] ActionDetailStat StatTempo, StatDamage, StatParryNegation, StatDodgeNegation, StatCrit;

	private CombatAction _action;
	public CombatAction Action {
		get => _action;
		set {
			_action = value;
			if (value is null) {
				Title.Text = "";
				StatContainer.Visible = false;
			}
			else {
				Title.Text = _action.Name;
				StatContainer.Visible = true;
				StatTempo.Value = _action.TempoCost;

				if (_action is AttackAction) {
					var attack = (_action as AttackAction).BaseAttack;

					StatDamage.Value = attack.DamageAmount;
					StatParryNegation.Value = attack.ParryNegation;
					StatDodgeNegation.Value = attack.DodgeNegation;
					StatCrit.Value = attack.CritBonus;

					StatDamage.Visible = true;
					StatParryNegation.Visible = true;
					StatDodgeNegation.Visible = true;
					StatCrit.Visible = true;
				}
				else {
					StatDamage.Visible = false;
					StatParryNegation.Visible = false;
					StatDodgeNegation.Visible = false;
					StatCrit.Visible = false;
				}
			}
		}
	}

	public override void _Process (double delta) {
		if (TurnManager.State != TurnManager.TurnState.Requesting) {
			Action = null;
		}
	}
}
