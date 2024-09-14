using System;
using Combat;

public class IsabelCharacter : Character {
    public override string IconFilePath => "res://combatants/characters/isabel/resources/icons/icon_profile.png";

    public override Type CombatantType => typeof (Isabel);
}