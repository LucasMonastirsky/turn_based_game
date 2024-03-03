using Combat;
using ResourceHelpers;

public partial class Miguel {
    private static string texture_path = "res://testers/miguel/textures";
	public class AnimationStore : StandardAnimationStore {
		public override SimpleAnimation Idle { get; set; }
        public override SimpleSprite Hurt { get; set; }
		public override SimpleSprite Dodge { get; set; }
		public override SimpleSprite Parry { get; set; }
		public override SimpleSprite Dead { get; set; }
		public SimpleSprite Swing { get; set; }
		public SimpleSprite Combo_1 { get; set; }
		public SimpleSprite Combo_2 { get; set; }
		public SimpleSprite Seal { get; set; }
    }
	protected override StandardAnimationStore StandardAnimations { get => Animations; }
	public AnimationStore Animations = new () {
		Idle = new SimpleAnimation() {
			Sprites = new SimpleSprite[] {
				new SimpleSprite(texture_path, "idle_0"),
				new SimpleSprite(texture_path, "idle_1"),
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
	};
}