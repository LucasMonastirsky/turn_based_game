using Combat;
using static Godot.TranslationServer;

public class TraitorsRing : Memento {
	public override string Name => Translate("Traitor's Ring");
	public override string Description => Translate("Gain 1 Rage at the end of each turn.");
	public override string Flavor => Translate("The memory of just considering it is enough to make her blood boil.");

	public override string IconFilePath => "res://combatants/characters/lara/resources/icons/mementos/icon_traitors_ring.png";

	public new Lara User => base.User as Lara;

	public override void Setup () {
		// TODO: this lol
	}

	public override void Clear () {

	}
}
