using Godot;

public partial class ActionDisplayButton : TextureButton {
	[Export] public Texture2D DefaultTexture;
	
	public void ResetTexture () {
		TextureNormal = DefaultTexture;
	}
}
