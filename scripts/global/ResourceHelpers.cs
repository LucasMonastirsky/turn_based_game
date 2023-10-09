using Godot;

namespace ResourceHelpers {
    public static class Resources {
        public static Texture2D LoadTexture (string folder_path, string texture_name) {
            return GD.Load<Texture2D>($"{folder_path}/{texture_name}.png");
        }
    }
}