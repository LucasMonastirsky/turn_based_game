using static Godot.TranslationServer;

public class LiquidCourage : Memento {
    public override string Name => Translate("Liquid Courage");
    public override string Description => Translate("Restores 10 HP temporarily");
    public override string Flavor => Translate("Makes you tell your friends that you love them");

    public override string IconFilePath => "res://combatants/characters/lara/resources/icons/mementos/icon_liquid_courage.png";

    public new Lara User => base.User as Lara;

    public override void Setup () {
        User.AddBonus(new (this, Combat.Stat.Hit, -2));
        User.RageBonus += 2;
    }

    public override void Clear () {
        User.RemoveBonusesFromSource(this);
        User.RageBonus -= 2;
    }
}