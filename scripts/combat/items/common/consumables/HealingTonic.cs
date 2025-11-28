using static Godot.TranslationServer;

public class HealingTonic : Item {
    public override string Name => Translate("Healing Tonic");
    public override string Description => Translate("Heal 10 Health");
    public override string Flavor => Translate("One would think it'd take a while to take effect.");

    public override string IconFilePath => "res://scripts/combat/items/icons/icon_healing_tonic.png";
}