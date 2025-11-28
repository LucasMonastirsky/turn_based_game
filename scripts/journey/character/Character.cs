using System;
using System.Collections.Generic;
using Combat;
using Development;

public class Character {
    public Type CombatantType { get; }
    public Combatant Combatant;

    public int Health, MaxHealth;

    public int Row;

    public Character (Type combatant_type) {
        if (!combatant_type.IsSubclassOf(typeof(Combatant))) Dev.Error("Tried to assign non-combatant type to character");

        CombatantType = combatant_type;
        Combatant = Activator.CreateInstance(CombatantType) as Combatant;
        Combatant.LoadResources();
    }
}