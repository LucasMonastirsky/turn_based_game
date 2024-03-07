using System;
using System.Linq;
using System.Threading.Tasks;
using Combat;
using Utils;

public partial class Hugo : Combatant {
    public override Type DefaultControllerType => typeof(PlayerController);

    public override string Name => "Hugo";

	private RollModifier parry_modifier;
	private RollModifier attack_modifier;

	protected override void Setup () {
		base.Setup();
		Actions = new ActionStore(this);

		
		AddRollModifier(parry_modifier = new (this, "Parry") { Bonus = 2 });
		AddRollModifier(attack_modifier = new (this, "Attack") { Bonus = 4 });
	}

	public override async Task Riposte (AttackResult attack_result) {
		if (attack_result.Attacker.Row == 0) {
			var move_targets = Positioner.GetMoveTargets(attack_result.Attacker, true);
			if (move_targets.Count > 0) {
				var empty_targets = move_targets.Where(target => target.Combatant is null);
				var final_move_targets = empty_targets.Count() > 0 ? empty_targets : move_targets;
				await Actions.Shove.Act(attack_result.Attacker, Positioner.SelectClosest(attack_result.Attacker, move_targets));
			}
			else {
				await Actions.Swing.Act(attack_result.Attacker);
			}
		}
	}
}
