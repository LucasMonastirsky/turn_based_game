using Combat;
using Godot;

public partial class CharacterDescription : PanelContainer {
	[Export] public Label LabelName;

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
			}
		}
	}

	public static CharacterDescription Current;
	public CharacterDescription () {
		Current = this;
	}
}
