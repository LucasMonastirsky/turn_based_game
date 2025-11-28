using System.Collections.Generic;
using System.Linq;
using Combat;
using Godot;

public partial class NavScreen : Control {
	[Export] Button TestButton;
	[Export] Container LowerIconContainer, UpperIconContainer;
	[Export] PackedScene CharacterIconScene;
	[Export] SlotContainer SkillContainer, ItemContainer;

	List<SlotButton> CharacterIcons = new ();

	public Combatant SelectedCombatant { get; private set; }

	public override void _Ready () {
		TestButton.Pressed += () => {
			List<List<Combatant>> ghouls = new () {
				new () {
					new Ghoul (),
					new Ghoul (),
				},
				new () {
					new Shooter (),
					new Boomer (),
					new Shooter (),
				}
			};
			ghouls.ForEach(list => list.ForEach(ghoul => ghoul.BaseMaxHealth = 20));
			Journey.StartBattle(ghouls);
		};
	}

	public void Load (List<Character> characters) {
		characters.ForEach(character => {
			character.Combatant.LoadIcons();

			var slot_button = CharacterIconScene.Instantiate<SlotButton>();
			
			if (character.Row == 0) UpperIconContainer.AddChild(slot_button);
			else LowerIconContainer.AddChild(slot_button);

			CharacterIcons.Add(slot_button);

			slot_button.Texture = character.Combatant.Icon;
			slot_button.OnPress = () => {
				SelectedCombatant = character.Combatant;
				CharacterIcons.ForEach(x => x.Selected = false);
				slot_button.Selected = true;
				CharacterDescription.Combatant = character.Combatant;
				SkillContainer.SetItems(character.Combatant.ActionList.Cast<SlotItem>().ToList());
				ItemContainer.SetItems(character.Combatant.Items.Cast<SlotItem>().ToList());
			};
		});

		CharacterDescription.Combatant = characters.FirstOrDefault().Combatant;
		CharacterIcons[0].Selected = true;
	}

	public void DeLoad () {
		QueueFree();
	}
}
