using Godot;
using System;
using Player.StateMachine;

namespace Player.StateMachine.States;

public partial class PlayerFallState : PlayerState
{
	[Export(PropertyHint.None, "suffix:px/s")] private float BaseMovementSpeed { get; set; } = 100f;
	[Export] private float FallGravityMultiplier { get; set; } = 1.165f;
	[Export] private float Acceleration { get; set; } = 8f;
	[Export] private float CoyoteTime { get; set; } = .125f;

	private float CoyoteTimer;
	private float MoveSpeed;

	public override void Init()
	{

	}

	public override void Enter()
	{
		GD.Print("Entered Fall State");
		Player.AnimPlayer.Play("fall");

		MoveSpeed = MathF.Max(BaseMovementSpeed, MathF.Abs(Player.Velocity.X));
		CoyoteTimer = CoyoteTime;
		Player.GravityMultiplier = FallGravityMultiplier;

		if (StateMachine.PreviousState == Jump)
		{
			CoyoteTimer = 0;
		}

	}

	public override void Exit()
	{
		GD.Print("Exited Fall State");

		Player.GravityMultiplier = 1f;
	}

	public override PlayerState HandleInput(InputEvent inputEvent)
	{
		if (CoyoteTimer > 0)
		{
			if (inputEvent.IsActionPressed("jump"))
			{
				return Jump;
			}
		}

		return null;
	}

	public override PlayerState Process(float delta)
	{
		return null;
	}

	public override PlayerState PhysicsProcess(float delta)
	{
		CoyoteTimer -= delta;

		Player.UpdateVelocity(Direction.X * MoveSpeed, Acceleration);

		if (Player.IsOnFloor())
		{
			return Idle;
		}

		return null;
	}
}
