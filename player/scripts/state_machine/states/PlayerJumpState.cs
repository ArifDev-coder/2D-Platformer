using Godot;
using System;
using Player.StateMachine;

namespace Player.StateMachine.States;

public partial class PlayerJumpState : PlayerState
{
	[Export] private float JumpVelocity { get; set; } = 350f;
	[Export] private AudioStream JumpAudio { get; set; }
	[Export(PropertyHint.None, "suffix:px/s")] private float BaseMovementSpeed { get; set; } = 100f;
	[Export] private float Acceleration { get; set; } = 8f;

	private float MoveSpeed;

	public override void Init()
	{

	}

	public override void Enter()
	{
		GD.Print("Entered Jump State");
		Player.AnimPlayer.Play("jump");

		Player.GlobalPosition = new Vector2(Player.GlobalPosition.X, Player.GlobalPosition.Y - 1);

		MoveSpeed = MathF.Max(BaseMovementSpeed, MathF.Abs(Player.Velocity.X));

		Player.Velocity = new Vector2(Player.Velocity.X, -JumpVelocity);

		Player.PlayAudio(JumpAudio);
	}

	public override void Exit()
	{
		GD.Print("Exited Jump State");
	}

	public override PlayerState HandleInput(InputEvent inputEvent)
	{
		if (inputEvent.IsActionReleased("jump"))
		{
			Player.Velocity = new Vector2(Player.Velocity.X, Player.Velocity.Y * .5f);

			return Fall;
		}

		return null;
	}

	public override PlayerState Process(float delta)
	{
		return null;
	}

	public override PlayerState PhysicsProcess(float delta)
	{
		Player.UpdateVelocity(Direction.X * MoveSpeed, Acceleration);

		if (Player.IsOnFloor())
		{
			return Idle;
		}
		else if (Player.Velocity.Y >= 0)
		{
			return Fall;
		}

		return null;
	}
}
