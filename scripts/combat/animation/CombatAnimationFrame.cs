using Godot;
using System;

namespace Combat {
	[Tool] public partial class CombatAnimationFrame : Resource {
		[Export] public float Duration { get; private set; }
		[Export] public Texture2D Texture { get; private set; }

		public CombatAnimationFrame () {
			
		}
	}
}
