using Combat;
using Godot;
using ResourceHelpers;
using Utils;

public abstract class Consumable : Source, SlotItem {
    private int _id = RNG.NewId;
    public int Id => _id;

    public abstract string Name { get; }
    public virtual string Description { get; } = "";
    public virtual string Flavor { get; } = "";

    public Combatant User { get; set; }

    public Texture2D IconTexture { get; private set; }
    public abstract string IconFilePath { get; }

    public Consumable () {
        IconTexture = Resources.LoadTexture(IconFilePath);
    }
}