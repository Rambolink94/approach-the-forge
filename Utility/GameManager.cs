using System.Collections.Generic;
using System.Linq;
using ApproachTheForge.Entities;
using ApproachTheForge.Entities.Golem;
using ApproachTheForge.Spawners;
using Godot;

namespace ApproachTheForge.Utility;

public partial class GameManager : Node2D
{
	[Export] private float _spawnRate = 1f;
	public ResourceManager ResourceManager { get; private set; }

	private Spawner<EnemyGolemAI> _golemSpawner;
	private int _wave;
	private float _timeSinceLastSpawn;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ResourceManager = GetNode<ResourceManager>("ResourceManager");
		_golemSpawner = GetNode<Spawner<EnemyGolemAI>>("GolumnSpawner");
		
		_golemSpawner.Spawn();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_timeSinceLastSpawn >= _spawnRate)
		{
			_timeSinceLastSpawn = 0;
			_golemSpawner.Spawn();
		}

		_timeSinceLastSpawn += (float)delta;
	}
}
