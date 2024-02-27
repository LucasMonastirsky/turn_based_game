using System;
using System.Linq;
using System.Threading.Tasks;
using Combat;
using Utils;

public partial class Hugo : Combatant {
    public override Type DefaultControllerType => typeof(PlayerController);

    public override string Name => "Hugo";

	protected override void Setup () {
		base.Setup();
		Actions = new ActionStore(this);
	}

	public override async Task Riposte (AttackResult attack_result) {
		if (attack_result.Attacker.Row == 0) {
			var move_targets = Positioner.GetMoveTargets(attack_result.Attacker);
			if (move_targets.Count > 0) {
				var empty_targets = move_targets.Where(target => target.Combatant is null);
				var final_move_targets = empty_targets.Count() > 0 ? empty_targets : move_targets;
				InteractionManager.QueueAction(Actions.Shove.Bind(attack_result.Attacker, Positioner.SelectClosest(attack_result.Attacker, move_targets)));
			}
			else {
				InteractionManager.QueueAction(Actions.Swing.Bind(attack_result.Attacker));
			}
		}
	}
}
