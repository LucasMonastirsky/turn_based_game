using Development;
using Godot;
using ResourceHelpers;

namespace Combat {
	public partial class SimpleSprite : Resource {
		public Texture2D Texture;
		public Vector2 Offset = new Vector2(0, 0);

		public SimpleSprite () {}
		public SimpleSprite (string folder_path, string texture_name, Vector2 offset = new Vector2()) {
			Texture = Resources.LoadTexture(folder_path, texture_name);
			Offset = offset;
		}
	}

	public partial class SimpleAnimation : Resource {
		public SimpleSprite[] Sprites;
		public double FrameDuration = 0.250;
	}

	public partial class CombatAnimator : Sprite2D {
		public bool Flipped { get => FlipH; set => FlipH = value; }

		private double elapsed_time;
		private int frame_index;

		private void SetSprite (SimpleSprite sprite) {
			if (sprite == null) {
                Dev.Error($"Null sprite in animator");
				return;
			}

			Texture = sprite.Texture;
			Position = sprite.Offset;
		}

		public void Play (SimpleSprite sprite) {
			current_animation = null;
			SetSprite(sprite);
		}

		[Export] private SimpleAnimation current_animation;
		public void Play (SimpleAnimation animation) {
			if (animation == current_animation) return;

			current_animation = animation;
			elapsed_time = 0;
			frame_index = 0;
		}

		public override void _Process (double delta) {
			if (current_animation == null) return;

			elapsed_time += delta;

			if (elapsed_time > current_animation.FrameDuration) {
				elapsed_time = 0;
				frame_index++;

                if (frame_index == current_animation.Sprites.Length) {
                    frame_index = 0;
                }

				SetSprite(current_animation.Sprites[frame_index]);
			}
		}
	}
}
