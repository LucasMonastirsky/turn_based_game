using System;
using System.Collections.Generic;
using Combat;
using Godot;
using ResourceHelpers;
using Utils;

public abstract class Item : Source, SlotItem {
    private int _id = RNG.NewId;
    public int Id => _id;

    public abstract string IconFilePath { get; }
    public Texture2D IconTexture { get; private set; }
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract string Flavor { get; }

    public List<Type> CombatantTypeRestrictions { get; }

    public Combatant User { get; set; }

    public Item () {
        IconTexture = Resources.LoadTexture(IconFilePath);
    }

    public virtual void Setup () {}
    public virtual void Clear () {}
}