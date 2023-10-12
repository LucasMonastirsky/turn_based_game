using Combat;

public partial class Hugo : StandardCombatant {
	public override string CombatName { get => "Hugo"; }

	protected override void Setup () {
		Actions = new ActionStore(this);
	}
}
