using Godot;

public interface SlotItem {
    Texture2D IconTexture { get; }

    string Name { get; }
    string Description { get; }
    string Flavor { get; }
}