using Godot;
using System;
using Player.StateMachine;

namespace Player.StateMachine.States;

public partial class PlayerCrouchState : PlayerState
{
	[Export] private float Deceleration { get; set; } = 4f;
	private CollisionShape2D CollisionShape2D;
	private CollisionShape2D CollisionShape2DCrouch;
	private RayCast2D RayCast2D;

	public override void Init()
	{
		CollisionShape2D = GetNode<CollisionShape2D>("../../CollisionShape2D");
		CollisionShape2DCrouch = GetNode<CollisionShape2D>("../../CollisionShape2D_Crouch");
		RayCast2D = GetNode<RayCast2D>("RayCast2D");

		RayCast2D.Enabled = false;
	}

	public override void Enter()
	{
		GD.Print("Entered Crouch State");
		Player.AnimPlayer.Play("crouch");

		CollisionShape2D.Disabled = true;
		CollisionShape2DCrouch.Disabled = false;
		RayCast2D.Enabled = true;
	}

	public override void Exit()
	{
		GD.Print("Exited Crouch State");

		CollisionShape2D.Disabled = false;
		CollisionShape2DCrouch.Disabled = true;
		RayCast2D.Enabled = false;
	}

	public override PlayerState HandleInput(InputEvent inputEvent)
	{
		if (inputEvent.IsActionPressed("jump"))
		{
			if (RayCast2D.IsColliding())
			{
				Player.Position = new Vector2(Player.Position.X, Player.Position.Y + 2);

				return Fall;
			}

			return Jump;
		}

		return null;
	}

	public override PlayerState Process(float delta)
	{
		return null;
	}

	public override PlayerState PhysicsProcess(float delta)
	{
		Player.UpdateVelocity(0, Deceleration);

		if (Direction.Y <= 0)
		{
			return Idle;
		}
		else if (!Player.IsOnFloor())
		{
			return Fall;
		}

		return null;
	}
}
