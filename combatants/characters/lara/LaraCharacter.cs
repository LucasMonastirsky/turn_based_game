using System;
using Combat;

public class LaraCharacter : Character {
    public override string IconFilePath => "res://combatants/characters/lara/resources/icons/icon_profile.png";

    public override Type CombatantType => typeof (Lara);
}