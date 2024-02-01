using Combat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class KeyboardController : Controller {
	public override void OnTurnStart () {
		CombatPlayerInterface.ShowActionList(Combatant.ActionList);
	}

	public override async Task<Combatant> RequestSingleTarget (Combatant user, TargetSelector selector) {
		var predicates = new List<Predicate<Combatant>> ();

		if (selector.Side != null) predicates.Add(x => (int) x.Side * (int) selector.Side == (int) user.Side);
		if (selector.Row != null) predicates.Add(x => x.Row == selector.Row);
		if (selector.Validator != null) predicates.Add(selector.Validator);

		var selectables = Battle.Combatants.All.Where(combatant => {
			foreach (var predicate in predicates) {
				if (!predicate(combatant)) return false;
			}

			return true;
		}).ToList();

		try {
			return await TargetingInterface.SelectSingleCombatant(selectables);
		}
		catch {
			return null;
		}
	}

	public override async Task<CombatPosition> RequestPosition (Combatant user) {
		var available_positions = Positioner.GetMoveTargets(user);

		return await TargetingInterface.SelectPosition(available_positions);
	}
}
