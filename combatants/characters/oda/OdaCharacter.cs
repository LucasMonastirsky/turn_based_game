using System;

public class OdaCharacter : Character {
    public override string IconFilePath => "res://combatants/characters/oda/resources/icons/icon_profile.png";

    public override Type CombatantType => typeof (Oda);
}