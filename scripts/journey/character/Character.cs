using System;
using System.Collections.Generic;
using Combat;
using Godot;

public abstract class Character {
    public abstract string IconFilePath { get; }

    public abstract Type CombatantType { get; }

    public int Health, MaxHealth;

    public int Row;

    public List<Memento> Mementos = new ();

    public Combatant Spawn () {
        var combatant = Activator.CreateInstance(CombatantType) as Combatant;
        combatant.Mementos = Mementos;
        return combatant;
    }
}