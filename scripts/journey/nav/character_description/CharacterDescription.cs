using Combat;
using Godot;

public partial class CharacterDescription : PanelContainer {
	[Export] public Label LabelName;
	[Export] StatIcon StatDamage, StatArmor, StatHit, StatCrit, StatParry, StatDodge;

	private static Combatant _combatant;
	public static Combatant Combatant {
		get => _combatant;
		set {
			_combatant = value;

			if (value == null) {
				Current.Visible = false;
			}
			else {
				Current.Visible = true;
				Current.LabelName.Text = value.Name;

				Current.StatDamage.Value = value.DamageBonus;
				Current.StatArmor.Value = value.Armor;
				Current.StatHit.Value = value.HitBonus;
				Current.StatCrit.Value = value.CritBonus;
				Current.StatParry.Value = value.ParryBonus;
				Current.StatDodge.Value = value.DodgeBonus;
			}
		}
	}

	public static CharacterDescription Current;
	public CharacterDescription () {
		Current = this;
	}
}
