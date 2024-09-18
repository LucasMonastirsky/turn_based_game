public class LiquidCourage : Memento {
    public override string Name => "Liquid Courage";

    public override string IconFilePath => null;

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