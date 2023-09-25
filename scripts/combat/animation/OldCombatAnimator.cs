using Godot;
using System;

namespace Combat {
	[Tool]
	public partial class OldCombatAnimator : Sprite2D {
		#region Debug
		[Export] private bool PlayInEditor = false;
		[Export] private bool PlayDebugAnimation {
			get => false;
			set { if (value) Play(DebugAnimation); }
		}
		[Export] private string DebugAnimation;
		#endregion

		#region Animation Storage
		[Export] private CombatAnimation[] animations;
		#endregion

		#region State
		[Export] public string CurrentAnimationName {
			get => current_animation == null ? "null" : current_animation.Name;
			set {}
		}
		[Export] private bool IsActive = true;
		private CombatAnimation current_animation;
		private CombatAnimationFrame current_frame { get => current_animation.Frames[frame_index]; }
		[Export] private int frame_index = 0;
		[Export] private float frame_time = 0;
		#endregion

		public void Play (string animation_name) { // TODO: use dictionary
			current_animation = Array.Find(animations, (item) => item.Name == animation_name);

			if (current_animation == null) {
				GD.PrintErr($"No animation found with name {animation_name} in animator of {GetParent()?.Name}");
			}

			frame_index = 0;
			frame_time = 0;
		}

        public override void _EnterTree() {
			if (animations.Length < 1) {
				GD.PrintErr($"Animator has no animations in animator of {GetParent()?.Name}");
				IsActive = false;
			}

			current_animation = animations[0];
        }

        public override void _Process(double delta) {
			if (Engine.IsEditorHint() && !PlayInEditor) return;

			if (!IsActive) return;

			if (current_animation == null) {
				GD.Print($"No current animation in animator of {GetParent()?.Name}");
				IsActive = false;
				return;
			}

			frame_time += (float) delta;

			if (frame_time >= current_frame.Duration / 1000) {
				frame_index++;

				if (frame_index >= current_animation.Frames.Length) {
					if (current_animation.IsLoop) frame_index = 0;
					else frame_index = current_animation.Frames.Length - 1;
				}

				frame_time = 0;
				Texture = current_frame.Texture;
			}
		}
	}
}
