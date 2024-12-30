using System.Collections.Generic;
using System.Linq;
using Combat;
using Godot;

public partial class Journey : Control {
	[Export] PackedScene NavScreenScene, BattleScene;

	private NavScreen NavScreen;
	private Battle Battle;

	public List<Combatant> Characters = new () {
		new Oda () { Position = new () { Row = 0 } },
		new Joseph () { Position = new () { Row = 0 } },
		new Lara () { Position = new () { Row = 0 }, Mementos = new () { new LiquidCourage (), new TraitorsRing (), new FamilyHeirloom () } },
		new Anna () { Position = new () { Row = 1 } },
		new Isabel () { Position = new () { Row = 1 } },
	};

	public static Journey Current;
	public Journey () {
		Current = this;
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
				Current.Characters.Where(combatant => combatant.Row == 0).ToList(),
				Current.Characters.Where(combatant => combatant.Row == 1).ToList(),
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
