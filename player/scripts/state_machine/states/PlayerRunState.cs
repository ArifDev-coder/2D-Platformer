using Godot;
using System;
using Player.StateMachine;

namespace Player.StateMachine.States;

public partial class PlayerRunState : PlayerState
{
	[Export(PropertyHint.None, "suffix:px/s")] private float Speed { get; set; } = 100f;
	[Export(PropertyHint.None, "suffix:px/s")] private float SprintSpeed { get; set; } = 150f;
	[Export] private float Acceleration { get; set; } = 4f;
	[Export] private float SkidAcceleration { get; set; } = 8f;
	[Export] private AudioStream SkidAudio { get; set; }

	private float CurrentAcceleration { get; set; }
	private float CurrentDirection { get; set; } = 0f;
	private float TargetSpeed { get; set; }

	public override void Init()
	{

	}

	public override void Enter()
	{
		GD.Print("Entered Run State");
		Player.AnimPlayer.Play("run");

		CurrentAcceleration = Acceleration;
		TargetSpeed = Speed;

		if (Input.IsActionPressed("action"))
		{
			TargetSpeed = SprintSpeed;
		}

		Player.AnimPlayer.CurrentAnimationChanged += OnAnimationChanged;

	}

	public override void Exit()
	{
		GD.Print("Exited Run State");

		Player.AnimPlayer.CurrentAnimationChanged -= OnAnimationChanged;
		Player.AudioStreamPlayer.Stop();

		Player.AnimPlayer.SpeedScale = 1;
	}

	public override PlayerState HandleInput(InputEvent inputEvent)
	{
		if (inputEvent.IsActionPressed("action"))
		{
			TargetSpeed = SprintSpeed;
		}
		else if (inputEvent.IsActionReleased("action"))
		{
			TargetSpeed = Speed;
		}
		else if (inputEvent.IsActionPressed("jump"))
		{
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
		if (!Player.IsOnFloor())
		{
			return Fall;
		}

		if (Direction.X == 0)
		{
			return Idle;
		}
		else if (Direction.Y > 0)
		{
			return Crouch;
		}
		else if (Mathf.Sign(Direction.X) == Mathf.Sign(Player.Velocity.X) || Player.Velocity.X == 0)
		{
			CurrentAcceleration = Acceleration;
			Player.AnimPlayer.Play("run");

			Player.AnimPlayer.SpeedScale = Mathf.Abs(Player.Velocity.X) / Speed;
		}
		else
		{
			CurrentAcceleration = SkidAcceleration;
			Player.AnimPlayer.Play("skid");
		}

		if (Direction.X != CurrentDirection)
		{
			CurrentDirection = Direction.X;
			Player.UpdateDirection(CurrentDirection);
		}

		Player.UpdateVelocity(Direction.X * TargetSpeed, CurrentAcceleration);

		return null;
	}

	public void OnAnimationChanged(StringName animName)
	{
		if (animName == "skid")
		{
			Player.PlayAudio(SkidAudio);
		}
		else
		{
			Player.AudioStreamPlayer.Stop();
		}

	}
}
