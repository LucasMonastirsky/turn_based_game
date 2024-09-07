using Combat;
using Godot;

public partial class ActionDetail : VBoxContainer {
	[Export] public Label Title;
	[Export] Control StatContainer;
	[Export] ActionDetailStat StatDamage, StatParryNegation, StatDodgeNegation, StatCrit;

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

				if (_action is AttackAction) {
					var attack = (_action as AttackAction).BaseAttack;

					StatContainer.Visible = true;

					StatDamage.Value = attack.DamageAmount;
					StatParryNegation.Value = attack.ParryNegation;
					StatDodgeNegation.Value = attack.DodgeNegation;
					StatCrit.Value = attack.CritBonus;
				}
				else StatContainer.Visible = false;
			}
		}
	}
}
