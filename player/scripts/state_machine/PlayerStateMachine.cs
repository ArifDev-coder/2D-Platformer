using Godot;
using Godot.Collections;
using Player.StateMachine.States;
using System;

namespace Player.StateMachine;

[Icon("res://player/sprites/icon_state_machine_16x16.png")]
public partial class PlayerStateMachine : Node2D
{
	public int StateSize { get; set; } = 3;

	private Array<PlayerState> States { get; set; } = new Array<PlayerState>();
	public PlayerState CurrentState => States.Count > 0 ? States[0] : null;
	public PlayerState PreviousState => States.Count > 1 ? States[1] : null;

	public Player Player { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Disabled;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		PlayerState.Direction = new Vector2(
			Input.GetAxis("left", "right"),
			Input.GetAxis("up", "down")
		);

		PlayerState NewState = CurrentState.Process((float)delta);
		ChangeState(NewState);
	}

	public override void _PhysicsProcess(double delta)
	{
		PlayerState NewState = CurrentState.PhysicsProcess((float)delta);
		ChangeState(NewState);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		PlayerState NewState = CurrentState.HandleInput(@event);
		ChangeState(NewState);
	}

	public void ChangeState(PlayerState newState)
	{
		if (newState == null)
		{
			return;
		}
		else if (newState == CurrentState)
		{
			return;
		}

		CurrentState?.Exit();

		States.Insert(0, newState);

		CurrentState.Enter();

		Error err = States.Resize(StateSize);

		if (err != Error.Ok)
		{
			GD.Print($"Resize Failed: {err}");
		}

		GD.Print($"Change State to {CurrentState}");
	}

	public void Init(Player player)
	{
		Player = player;
		States.Clear();

		foreach (var child in GetChildren())
		{
			if (child is PlayerState playerState)
			{
				States.Add(playerState);
			}
		}

		GD.Print($"Init Found {States.Count} states");

		if (States.Count == 0)
		{
			return;
		}

		PlayerState.Player = player;
		PlayerState.StateMachine = this;

		foreach (PlayerState state in States)
		{
			state.Init();
		}

		ChangeState(CurrentState.Crouch);
		ChangeState(PreviousState.Idle);

		ProcessMode = ProcessModeEnum.Inherit;
	}
}
