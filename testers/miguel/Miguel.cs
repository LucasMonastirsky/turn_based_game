using Combat;
using Godot;
using System;

public partial class Miguel : Node {
	private NewCombatAnimator animator;
	private static string texture_path = "res://testers/miguel/animation/textures";
	private SimpleAnimation Idle = new SimpleAnimation () {
		FrameDuration = 0.25f,
		Sprites = new SimpleSprite[] {
			new SimpleSprite () {
				Texture = GD.Load<Texture2D>($"{texture_path}/idle_0.png"),
				Offset = new Vector2 (),
			},
			new SimpleSprite () {
				Texture = GD.Load<Texture2D>($"{texture_path}/idle_1.png"),
				Offset = new Vector2 (),
			},
		}
	};
	private SimpleSprite Parry = new SimpleSprite () {
		Texture = GD.Load<Texture2D>($"{texture_path}/parry.png"),
		Offset = new Vector2() { X = 0, Y = 0, },
	};

	public override void _Ready () {
		animator = GetNode<NewCombatAnimator>("Animator");
		animator.Play(Idle);
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
