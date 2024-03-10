using Godot;

namespace ResourceHelpers {
    public static class Resources {
        public static Texture2D LoadTexture (string folder_path, string texture_name) {
            return GD.Load<Texture2D>($"{folder_path}/{texture_name}.png");
        }

        public static AudioStream LoadAudio (string folder_path, string file_name) {
            return GD.Load<AudioStream>($"{folder_path}/{file_name}");
        }
    }
}