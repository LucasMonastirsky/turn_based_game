using CustomDebug;
using Godot;

namespace Combat {
	public partial class Battle : Node {
		private static Battle current;
		public static Battle Current { get => current; }

		private ICombatant[] combatants;
		public ICombatant[] Combatants { get => combatants; }
		public IPositioner Positioner { get; protected set; }

		public override void _Ready () {
			Battle.current = this;

			Positioner = GetNode<StandardPositioner>("Positioner"); // TODO: uhhhhhhh

			combatants = new ICombatant[] {
				GetNode("Hugo") as ICombatant,
				GetNode("Sasuke") as ICombatant,
			};

			combatants[0].LoadIn(new Position { Side = Side.Left, Row = 0, RowPos = 0, });
			combatants[1].LoadIn(new Position { Side = Side.Right, Row = 0, RowPos = 0, });
		}
	}
}
