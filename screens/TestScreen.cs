using Combat;
using Godot;
using System;
using System.Collections.Generic;
using Utils;

public partial class TestScreen : Node2D {
	[Export] PackedScene BattleScene;

	[Export] Container ButtonContainer;
	[Export] Button Random, Balanced, Ghoul;

	List<List<Combatant>> PlayerCombatants => new () {
		new () { new Joseph (), new Lara (), },
		new () { new Oda (), new Anna (), new Isabel (), },
	};

	void CreateBattle (List<List<Combatant>> enemies) {
		ButtonContainer.Visible = false;

		var battle = BattleScene.Instantiate() as StandardBattle;
		AddChild(battle);

		battle.LoadCharacters(
			PlayerCombatants,
			enemies
		);

		battle.Start();
	}

	public override void _EnterTree () {
		Balanced.Pressed += () => {
			CreateBattle(new () {
				new () { new Ghoul (), new Ghoul (), },
				new () { new Bird (), new Shooter (), new Bird (), },
			});
		};

		Random.Pressed += () => {
			var enemies = new List<List<Combatant>> () { new (), new (), };
			var types = new List<Type> () { typeof (Ghoul), typeof (Bird), typeof (Shooter), typeof (Boomer) };

			for (var i = 0; i < 5; i++) {
				var enemy = Activator.CreateInstance(RNG.SelectFrom(types)) as Combatant;

				if (i < 2) enemies[0].Add(enemy);
				else enemies[1].Add(enemy);
			}

			CreateBattle(enemies);
		};

		Ghoul.Pressed += () => {
			CreateBattle(new () {
				new () { new Ghoul (), new Ghoul (), },
				new () { new Ghoul (), new Ghoul (), new Ghoul (), },
			});
		};
	}
}
