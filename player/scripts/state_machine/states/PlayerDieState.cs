using Godot;
using System;

namespace PlayerNode.StateMachine.States;

public partial class PlayerDieState : PlayerState
{
	[Export] public float FreezeTime = .4f;
	[Export] public float PopForce = -350f;
	[Export] public float TotalTime = 2f;

	private float _timer;
	private float _elapsed;
	private bool _popped;
	private bool _finished;

	public override void Init()
	{
	}

	public override void Enter()
	{
		_timer = FreezeTime;
		_elapsed = 0f;
		_popped = false;
		_finished = false;

		Player.Velocity = Vector2.Zero;

		Player.AnimPlayer.Play("run");
		Player.AnimPlayer.Pause();

		Player.GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred("disabled", true);

		Player.GetNodeOrNull<Camera2D>("Camera2D").PositionSmoothingEnabled = false;
		Player.GetNodeOrNull<Camera2D>("Camera2D").Reparent(Player.GetParent());

		Player.ZIndex = 100;
	}

	public override void Exit()
	{
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
		_elapsed += delta;

		if (!_popped)
		{
			_timer -= delta;

			if (_timer <= 0f)
			{
				_popped = true;
				Player.Velocity = new Vector2(0, PopForce);
			}
			return null;
		}

		Player.Velocity += new Vector2(0, Player.GetGravity().Y * delta);

		if (!_finished && _elapsed >= TotalTime)
		{
			_finished = true;
			Player.GetTree().ReloadCurrentScene();
		}

		return null;
	}
}
