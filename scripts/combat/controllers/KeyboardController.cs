using Combat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class KeyboardController : Controller {
	public override void OnTurnStart () {
		CombatPlayerInterface.ShowActionList(Combatant.ActionList);
	}

	public override async Task<ICombatant> RequestSingleTarget (ICombatant user, TargetSelector selector) {
		var predicates = new List<Predicate<ICombatant>> ();

		if (selector.Side != null) predicates.Add(x => (int) x.Side * (int) selector.Side == (int) user.Side);
		if (selector.Row != null) predicates.Add(x => x.Row == selector.Row);
		if (selector.Validator != null) predicates.Add(selector.Validator);

		var selectables = Battle.Current.Combatants.Where(combatant => {
			foreach (var predicate in predicates) {
				if (!predicate(combatant)) return false;
			}

			return true;
		}).ToList();

		return await TargetingInterface.SelectSingleCombatant(selectables);
	}

	public override async Task<CombatPosition> RequestPosition (ICombatant user) {
		var available_positions = Positioner.GetMoveTargets(user);

		return await TargetingInterface.SelectPosition(available_positions);
	}
}
