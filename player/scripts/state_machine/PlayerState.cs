using Godot;
using PlayerNode.StateMachine.States;
using System;

namespace PlayerNode.StateMachine;

public partial class PlayerState : Node2D
{
	public static Player Player { get; set; }
	public static PlayerStateMachine StateMachine { get; set; }
	public static Vector2 Direction { get; set; }

	public PlayerIdleState Idle { get; set; }
	public PlayerRunState Run { get; set; }
	public PlayerJumpState Jump { get; set; }
	public PlayerFallState Fall { get; set; }
	public PlayerCrouchState Crouch { get; set; }
	public PlayerDieState Die { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Idle = GetNode<PlayerIdleState>("%Idle");
		Run = GetNode<PlayerRunState>("%Run");
		Jump = GetNode<PlayerJumpState>("%Jump");
		Fall = GetNode<PlayerFallState>("%Fall");
		Crouch = GetNode<PlayerCrouchState>("%Crouch");
		Die = GetNode<PlayerDieState>("%Die");
	}

	public virtual void Init()
	{

	}

	public virtual void Enter()
	{

	}

	public virtual void Exit()
	{

	}

	public virtual PlayerState HandleInput(InputEvent inputEvent)
	{
		return null;
	}

	public virtual PlayerState Process(float delta)
	{
		return null;
	}

	public virtual PlayerState PhysicsProcess(float delta)
	{
		return null;
	}

}
