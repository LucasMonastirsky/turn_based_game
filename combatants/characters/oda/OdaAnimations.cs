using Combat;

namespace Combat {
	public partial class Oda {
		public override string resources_path => "res://combatants/characters/oda/resources";

		public class AnimationStore : StandardAnimationStore {
			public SimpleAnimation SheathedIdle { get; set; }
            public SimpleSprite Swing { get; set; }
			public SimpleSprite Combo_1 { get; set; }
			public SimpleSprite Combo_2 { get; set; }
			public SimpleSprite Seal { get; set; }
			public SimpleSprite Throw { get; set; }
		}

		public override AnimationStore Animations => _animations;
        private AnimationStore _animations;

		protected override void LoadSprites () {
			_animations = new () {
				Idle = new SimpleAnimation() {
					Sprites = new SimpleSprite[] {
						LoadSprite("idle_sword"),
						LoadSprite("idle_sword"),
					}
				},
				SheathedIdle = new SimpleAnimation() {
					Sprites = new SimpleSprite[] {
						LoadSprite("idle_sheath_0"),
						LoadSprite("idle_sheath_1"),
					}
				},
				Hurt = LoadSprite("hurt"),
				Parry = LoadSprite("parry"),
				Dodge = LoadSprite("dodge"),
				Dead = LoadSprite("dead"),
				Swing = LoadSprite("swing"),
				Combo_1 = LoadSprite("combo_1"),
				Combo_2 = LoadSprite("combo_2"),
				Seal = LoadSprite("seal"),
				Throw = LoadSprite("throw"),
			};
		}
	}
}