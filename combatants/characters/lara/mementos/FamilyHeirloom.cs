using static Godot.TranslationServer;

public class FamilyHeirloom : Item {
	public override string Name => Translate("Family Heirloom");
	public override string Description => Translate("+2 dodge bonus.");
	public override string Flavor => Translate("Had she sold it, she wouldn't have had to work a single day.");

	public override string IconFilePath => "res://combatants/characters/lara/resources/icons/mementos/icon_family_heirloom.png";

	public new Lara User => base.User as Lara;

	public override void Setup () {
		// TODO: this lol
	}

	public override void Clear () {

	}
}
