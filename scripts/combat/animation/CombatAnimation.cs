using Godot;
using System;

namespace Combat {
	[Tool] public partial class CombatAnimation : Resource {
		[Export] public string Name { get; set; }
		[Export] public bool IsLoop { get; private set; }
		[Export] public CombatAnimationFrame[] Frames { get; private set; }

		public CombatAnimation () {

		}
	}

}
