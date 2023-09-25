using CustomDebug;
using Godot;
using System;
using System.Linq;

namespace Combat {
	public partial class Battle : Node {
		private static Battle current;
		public static Battle Current { get => current; }

		private ICombatant[] combatants;

		public override void _Ready () {
			Battle.current = this;

			combatants = new ICombatant[] {
				GetNode("Hugo") as ICombatant,
				GetNode("Tobi") as ICombatant,
			};

			combatants[0].Side = Side.Left;
			combatants[1].Side = Side.Right;

			foreach (var x in combatants) {
				Dev.Log(x.CombatName);
			}
		}
	}
}
