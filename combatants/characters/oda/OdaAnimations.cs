using Combat;

namespace Combat {
	public partial class Oda {
		private static string texture_path = "res://combatants/characters/oda/textures";
		public class AnimationStore : StandardAnimationStore {
			public SimpleAnimation SheathedIdle { get; set; }
            public SimpleSprite Swing { get; set; }
			public SimpleSprite Combo_1 { get; set; }
			public SimpleSprite Combo_2 { get; set; }
			public SimpleSprite Seal { get; set; }
			public SimpleSprite Throw { get; set; }
		}

		public override StandardAnimationStore StandardAnimations { get => Animations; }
		public AnimationStore Animations = new () {
			Idle = new SimpleAnimation() {
				Sprites = new SimpleSprite[] {
					new SimpleSprite(texture_path, "idle_sword"),
					new SimpleSprite(texture_path, "idle_sword"),
				}
			},
			SheathedIdle = new SimpleAnimation() {
				Sprites = new SimpleSprite[] {
					new SimpleSprite(texture_path, "idle_sheath_0"),
					new SimpleSprite(texture_path, "idle_sheath_1"),
				}
			},
			Hurt = new SimpleSprite(texture_path, "hurt"),
			Parry = new SimpleSprite(texture_path, "parry"),
			Dodge = new SimpleSprite(texture_path, "dodge"),
			Dead = new SimpleSprite(texture_path, "dead"),
			Swing = new SimpleSprite(texture_path, "swing"),
			Combo_1 = new SimpleSprite(texture_path, "combo_1"),
			Combo_2 = new SimpleSprite(texture_path, "combo_2"),
			Seal = new SimpleSprite(texture_path, "seal"),
			Throw = new SimpleSprite(texture_path, "throw"),
		};
	}
}