using Godot;
using System;
using Player.StateMachine;

namespace Player.StateMachine.States;

public partial class PlayerCrouchState : PlayerState
{

	public override void Init()
	{

	}

	public override void Enter()
	{
		GD.Print("Entered Crouch State");
	}

	public override void Exit()
	{
		GD.Print("Exited Crouch State");
	}

	public override PlayerState HandleInput(InputEvent inputEvent)
	{
		return null;
	}

	public override PlayerState Process(float delta)
	{
		return null;
	}

	public override PlayerState PhysicsProcess(float delta)
	{
		return null;
	}
}
