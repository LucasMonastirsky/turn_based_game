using System.Collections.Generic;
using System.Linq;
using Combat;
using Godot;
using ResourceHelpers;

public partial class NavScreen : Control {
	[Export] Button TestButton;
	[Export] Container LowerIconContainer, UpperIconContainer;
	[Export] PackedScene CharacterIconScene;

	public override void _Ready () {
		TestButton.Pressed += () => {
			Journey.StartBattle(new () {
				new Ghoul (),
				new Ghoul (),
			});
		};
	}

	public void Load (List<Character> characters) {
		characters.ForEach(character => {
			var icon = CharacterIconScene.Instantiate<CharacterIcon>();
			
			if (character.Row == 0) UpperIconContainer.AddChild(icon);
			else LowerIconContainer.AddChild(icon);

			icon.Texture = Resources.LoadTexture(character.IconFilePath);
		});
	}
}
