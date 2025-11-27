using System;
using System.Collections.Generic;
using Combat;

public partial class Lara {
	public override List<CombatAction> ActionList => new () {
		Actions.Chop, Actions.Sweep, Actions.Charge, Actions.Unleash, Actions.Impatience, Actions.Relax, Actions.Move, Actions.Pass,
	};

	public ActionStore Actions;

	public class ActionStore {
		public ActionClasses.Chop Chop;
		public ActionClasses.Sweep Sweep;
		public ActionClasses.Charge Charge;
		public ActionClasses.Unleash Unleash;
		public ActionClasses.Impatience Impatience;
		public ActionClasses.Relax Relax;

		public CommonActions.Move Move;
		public CommonActions.Pass Pass;

		public ActionStore (Lara hidan) {
			foreach (var field in typeof(ActionStore).GetFields()) {
				field.SetValue(this, Activator.CreateInstance(field.FieldType, hidan));
			}
		}
	}
}
