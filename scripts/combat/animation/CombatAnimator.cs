using CustomDebug;
using Godot;
using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace Combat {
	public partial class CombatAnimator : Sprite2D {
		#region State
		private bool _flipped;
		public bool Flipped {
			get => _flipped;
			set { _flipped = FlipH = value; }
		}
		private CombatAnimation current_animation;
		private CombatAnimationFrame current_frame { get => current_animation.Frames[frame_index]; }
		[Export] private bool IsActive = true;
		[Export] private int frame_index = 0;
		[Export] private float frame_time = 0;
		#endregion

		private EventManager event_manager;

		public class EventManager {
			public EventManager (CombatAnimation animation) {
				this.frames = new Frame[animation.Frames.Length];
				
				for (var i = 0; i < frames.Length; i++) {
					this.frames[i] = new Frame(this);
				}
			}

			private TaskCompletionSource task_completion_source = new ();
			public Task Task { get => task_completion_source.Task; }

			public delegate void EventHandler ();
			private EventHandler[] on_end_handlers = new EventHandler[] {};
			public EventManager OnEnd (EventHandler handler) {
				on_end_handlers = on_end_handlers.Append(handler).ToArray();
				return this;
			}
			public void TriggerEnd () {
				task_completion_source.TrySetResult();
				foreach (var handler in on_end_handlers) {
					handler();
				}
			}

			private Frame[] frames;
			public Frame[] Frames { get => frames; }
			public class Frame {
				private EventManager event_manager;

				public Frame (EventManager event_manager) {
					this.event_manager = event_manager;
				}

				private TaskCompletionSource task_completion_source = new ();
				public Task Task { get => task_completion_source.Task; }

				private EventHandler[] on_end_handlers = new EventHandler[] {};
				public EventManager OnEnd (EventHandler handler) {
					on_end_handlers = on_end_handlers.Append(handler).ToArray();
					return event_manager;
				}
				public void TriggerEnd () {
					task_completion_source.TrySetResult();
					foreach (var handler in on_end_handlers) {
						handler();
					}
				}
			}
		}

		public EventManager Play (CombatAnimation animation) {
			if (animation == null) {
				Dev.Error($"Received null animation in animator of {GetParent()?.Name}");
				throw new Exception($"Null animation in {GetParent()?.Name}");
			}

			Dev.Log(Dev.TAG.ANIMATION, "Playing animation: " + animation.Name);

			event_manager = new EventManager(animation);
			current_animation = animation;
			Texture = current_animation.Frames[0].Texture;
			frame_index = 0;
			frame_time = 0;

			return event_manager;
		}

        public override void _Process(double delta) {
			if (!IsActive) return;

			if (current_animation == null) {
				Dev.Error($"No current animation in animator of {GetParent()?.Name}");
				IsActive = false;
				return;
			}

			frame_time += (float) delta;

			if (frame_time >= current_frame.Duration / 1000) {
				event_manager.Frames[frame_index].TriggerEnd();

				frame_index++;

				if (frame_index >= current_animation.Frames.Length) {
					event_manager.TriggerEnd(); // TODO: trigger only once

					if (current_animation.IsLoop) frame_index = 0;
					else frame_index = current_animation.Frames.Length - 1;
				}

				frame_time = 0;
				Texture = current_frame.Texture;
			}
		}
	}
}
