using Godot;
using System;

namespace Player.StateMachine.States;

public partial class PlayerIdleState : PlayerState
{
	[Export] public float Deceleration { get; set; } = 8f;

	public override void Init()
	{

	}

	public override void Enter()
	{
		GD.Print("Entered Idle State");
		Player.AnimPlayer.Play("idle");
	}

	public override void Exit()
	{
		GD.Print("Exited Idle State");
	}

	public override PlayerState HandleInput(InputEvent inputEvent)
	{
		if (inputEvent.IsActionPressed("jump"))
		{
			return Jump;
		}

		return null;
	}

	public override PlayerState Process(float delta)
	{	
		if (Direction.X != 0)
		{
			return Run;
		}

		return null;
	}

	public override PlayerState PhysicsProcess(float delta)
	{
		Player.UpdateVelocity(0, Deceleration);

		if (!Player.IsOnFloor())
		{
			return Fall;
		}

		return null;
	}
}
