using Godot;
using System;

namespace Enemy;

public partial class Enemy : CharacterBody2D
{
	[Export] public float Speed { get; set; } = 10.0f;
	[Export] public float Direction { get; set; } = -1f;
	// [Export] public float Acceleration { get; set; } = 5f;

	public AnimationPlayer AnimPlayer { get; set; }
	public Timer Timer { get; set; }

	public override void _Ready()
	{
		AnimPlayer = GetNode<AnimationPlayer>("Animation/AnimationPlayer");
		Timer = GetNode<Timer>("Timer");
	}

	public override void _PhysicsProcess(double delta)
	{
		// Add the gravity.
		if (!IsOnFloor())
		{
			Velocity += GetGravity() * (float)delta;
		}

		Velocity = new Vector2(Direction * Speed, Velocity.Y);

		MoveAndSlide();
	}

	public void _on_timer_timeout()
	{
		Direction *= -1;

		if (Direction == 1)
		{
			AnimPlayer.Play("Walk_flip");
		}
		else if (Direction == -1)
		{
			AnimPlayer.Play("Walk");
		}
	}
}
