using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Combat {
	public partial class Battle : Node {
        public static CombatantStore Combatants { get; protected set; }
		
		public static Battle Current;

		public Battle () {
			Current = this;
		}

		public static void RegisterCombatantNode (Node2D node) {
			Current.AddChild(node);
		}

		public void LoadCombatants (List<List<Combatant>> left_combatants, List<List<Combatant>> right_combatants) {
			Combatants = new (
				new List<Combatant> ().Concat(left_combatants[0]).Concat(left_combatants[1]).Concat(right_combatants[0]).Concat(right_combatants[1])
			);

			foreach (var combatant in Combatants) {
				combatant.LoadIn();
			}

			left_combatants[0].ForEach(combatant => combatant.Position = new () { Side = Side.Left, Row = 0, });
			left_combatants[1].ForEach(combatant => combatant.Position = new () { Side = Side.Left, Row = 1, });

			right_combatants[0].ForEach(combatant => combatant.Position = new () { Side = Side.Right, Row = 0, });
			right_combatants[1].ForEach(combatant => combatant.Position = new () { Side = Side.Right, Row = 1, });
		}

		public void Start () {
			CommonSounds.Load();
			Positioner.Setup();
			RoundManager.Begin();
			TurnManager.BeginLoop();
		}
	}
}
