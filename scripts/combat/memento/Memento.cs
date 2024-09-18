using System;
using Combat;
using Utils;

public abstract class Memento : Source {
    private int _id = RNG.NewId;
    public int Id => _id;

    public abstract string Name { get; }
    public abstract string IconFilePath { get; }

    public Type CombatantType { get; }

    public Combatant User { get; set; }

    public virtual void Setup () {}
    public virtual void Clear () {}
}