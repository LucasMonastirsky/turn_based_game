using System;
using System.Collections.Generic;
using System.Linq;
using Combat;
using Development;
using Godot;

public partial class Journey : Control {
	[Export] PackedScene NavScreenScene, BattleScene;

	private NavScreen NavScreen;
	private Battle Battle;

	public List<Character> Characters = new () {
		new (typeof(Oda)) { Row = 0 },
		new (typeof(Joseph)) { Row = 0 },
		new (typeof(Lara)) { Row = 0, },
		new (typeof(Isabel)) { Row = 1 },
		new (typeof(Anna)) { Row = 1 },
	};

	public static Journey Current;
	public Journey () {
		Current = this;

		Characters.Find(c => c.Combatant.Name == "Lara").Combatant.Items = new () { new LiquidCourage (), new TraitorsRing (), new FamilyHeirloom (), new HealingTonic () };
	}

	public override void _Ready () {
		NavScreen = NavScreenScene.Instantiate<NavScreen>();
		NavScreen.Load(Characters);
		AddChild(NavScreen);
	}

	public static void StartBattle (List<List<Combatant>> enemies) {
		Current.Battle = Current.BattleScene.Instantiate<Battle>();
		
		Current.AddChild(Current.Battle);

		Current.Battle.LoadCombatants(
			new List<List<Combatant>> () {
				Current.Characters.Where(character => character.Row == 0).Select(character => character.Combatant).ToList(),
				Current.Characters.Where(character => character.Row == 1).Select(character => character.Combatant).ToList(),
			},
			enemies
		);

		Current.NavScreen.DeLoad();
		Current.Battle.Start();
	}

	public static void EndBattle () {
		Current.NavScreen = Current.NavScreenScene.Instantiate<NavScreen>();
		Current.NavScreen.Load(Current.Characters);
		Current.AddChild(Current.NavScreen);

		Current.Battle.QueueFree();
	}
}
