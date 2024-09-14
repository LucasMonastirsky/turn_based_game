using System;
using Combat;

public class AnnaCharacter : Character {
    public override string IconFilePath => "res://combatants/characters/anna/resources/icons/icon_profile.png";

    public override Type CombatantType => typeof (Anna);
}