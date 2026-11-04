using Godot;
using System;
using EnemyNode;
using PlayerNode;

public partial class Main : Node2D
{
	[Export] public Node2D Level { get; set; }

	public override void _Ready()
	{
		SetupLevel();
	}

	private void SetupLevel()
	{
		var Enemies = Level.GetNodeOrNull("Enemies");

		if (Enemies != null)
		{
			foreach (var enemy in Enemies.GetChildren())
			{
				if (enemy is Enemy e) e.PlayerDied += OnPlayerDied;
			}
		}
	}

	private void OnPlayerDied(Node2D body)
	{
		if (body is Player player)
		{
			player.PlayerDie();
		}
	}
}
