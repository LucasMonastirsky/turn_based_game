using System;
using Combat;

public partial class Shooter : Combatant {
    public override string Name => "Shooter";

    public override Type DefaultControllerType => typeof (ShooterController);

    protected override void Setup () {
        base.Setup();

        Actions = new (this);

        BaseMaxHealth = 20;
        BaseParryBonus = 5;
        BaseDodgeBonus = 7;
        BaseInitiativeBonus = 5;
    }
}