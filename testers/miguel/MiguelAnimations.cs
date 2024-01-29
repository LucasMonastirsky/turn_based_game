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
    }
	protected override StandardAnimationStore StandardAnimations { get => Animations; }
	public AnimationStore Animations = new () {
		Idle = new SimpleAnimation() {
			Sprites = new SimpleSprite[] {
				new SimpleSprite() {
					Texture = Resources.LoadTexture(texture_path, "idle_0"),
				},
				new SimpleSprite() {
					Texture = Resources.LoadTexture(texture_path, "idle_1"),
				},
			}
		},
		Hurt = new SimpleSprite() {
			Texture = Resources.LoadTexture(texture_path, "hurt"),
		},
		Parry = new SimpleSprite() {
			Texture = Resources.LoadTexture(texture_path, "parry"),
		},
		Dodge = new SimpleSprite() {
			Texture = Resources.LoadTexture(texture_path, "dodge"),
		},
		Dead = new SimpleSprite() {
			Texture = Resources.LoadTexture(texture_path, "dead"),
		},
		Swing = new SimpleSprite() {
			Texture = Resources.LoadTexture(texture_path, "swing"),
		},
	};
}