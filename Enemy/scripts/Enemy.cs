using Godot;
using System;

namespace EnemyNode;

public partial class Enemy : CharacterBody2D
{
	[Signal] public delegate void PlayerDiedEventHandler(Node2D body);

	[Export] public float Speed { get; set; } = 10.0f;
	[Export] public float Direction { get; set; } = -1f;
	// [Export] public float Acceleration { get; set; } = 5f;

	public AnimationPlayer AnimPlayer { get; set; }
	public Sprite2D Sprite { get; set; }
	public Timer Timer { get; set; }

	public override void _Ready()
	{
		AnimPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		Sprite = GetNode<Sprite2D>("Sprite");
		Timer = GetNode<Timer>("Timer");

		GetNode<Area2D>("Area2D").BodyEntered += OnBodyEntered;

		Sprite.Scale = new Vector2(1, 1);
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
		Sprite.Scale = new Vector2(Direction * -1, 1);

		// if (Direction == 1)
		// {
		// 	AnimPlayer.Play("Walk_flip");
		// }
		// else if (Direction == -1)
		// {
		// 	AnimPlayer.Play("Walk");
		// }
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body.Name == "Player")
		{
			EmitSignal("PlayerDied", body);
		}

	}
}
