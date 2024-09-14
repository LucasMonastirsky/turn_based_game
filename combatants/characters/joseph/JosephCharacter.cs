using System;
using Combat;

public class JosephCharacter : Character {
    public override string IconFilePath => "res://combatants/characters/joseph/resources/icons/icon_profile.png";

    public override Type CombatantType => typeof (Joseph);
}