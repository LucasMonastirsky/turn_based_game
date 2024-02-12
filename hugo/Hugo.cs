using System;
using Combat;

public partial class Hugo : Combatant {
    public override Type DefaultControllerType => typeof(PlayerController);

    public override string CombatName => "Hugo";

	protected override void Setup () {
		base.Setup();
		Actions = new ActionStore(this);
	}
}
