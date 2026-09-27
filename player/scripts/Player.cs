using Godot;
using Player.StateMachine;
using System;

namespace Player;

public partial class Player : CharacterBody2D
{
	public float GravityMultiplier { get; set; } = 1f;

	public Sprite2D Sprite { get; set; }
	public AnimationPlayer AnimPlayer { get; set; }
	public AudioStreamPlayer2D AudioStreamPlayer { get; set; }
	public PlayerStateMachine PlayerStateMachine { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Sprite = GetNode<Sprite2D>("Sprite2D");
		AnimPlayer = GetNode<AnimationPlayer>("Sprite2D/AnimationPlayer");
		AudioStreamPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
		PlayerStateMachine = GetNode<PlayerStateMachine>("PlayerStateMachine");

		PlayerStateMachine.Init(this);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!IsOnFloor())
		{
			Velocity += GetGravity() * (float)delta * GravityMultiplier;
		}

		MoveAndSlide();
	}

	public void UpdateVelocity(float velocity, float acceleration)
	{
		Velocity = new Vector2(
			Mathf.MoveToward(Velocity.X, velocity, acceleration),
			Velocity.Y
		);
	}

	public void PlayAudio(AudioStream audio)
	{
		if (audio == null)
		{
			return;
		}

		AudioStreamPlayer.Stream = audio;
		AudioStreamPlayer.Play();
	}

	public void UpdateDirection(float direction)
	{
		if (direction < 0)
		{
			Sprite.Scale = new Vector2(
				-1,
				Sprite.Scale.Y
			);
		}
		else
		{
			Sprite.Scale = new Vector2(
				1,
				Sprite.Scale.Y
			);
		}
	}
}
