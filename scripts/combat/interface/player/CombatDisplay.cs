using Godot;

namespace Combat {
	public partial class CombatDisplay : Control {
		private static ActionDisplay ActionDisplay;

		public override void _EnterTree () {
			ActionDisplay = new ();
		}
	}
}
