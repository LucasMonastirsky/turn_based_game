using System;
using Godot;

namespace Combat {
	public partial class SimpleSprite : Resource {
		public Texture2D Texture;
		public Vector2 Offset;
	}

	public partial class SimpleAnimation : Resource {
		public SimpleSprite[] Sprites;
		public double FrameDuration;
	}

	public partial class NewCombatAnimator : Sprite2D {
		private double elapsed_time;
		private int frame_index;

		private void SetSprite (SimpleSprite sprite) {
			Texture = sprite.Texture;
			Position = sprite.Offset;
		}

		public void Play (SimpleSprite sprite) {
			current_animation = null;
			SetSprite(sprite);
		}

		[Export] private SimpleAnimation current_animation;
		public void Play (SimpleAnimation animation) {
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
