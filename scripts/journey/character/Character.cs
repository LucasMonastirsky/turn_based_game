using System;
using Combat;
using Godot;

public abstract class Character {
    public abstract string IconFilePath { get; }

    public abstract Type CombatantType { get; }

    public int Health, MaxHealth;

    public int Row;

    public Combatant Spawn () {
        return Activator.CreateInstance(CombatantType) as Combatant;
    }
}