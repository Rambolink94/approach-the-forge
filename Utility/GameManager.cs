using ApproachTheForge.Entities.Player;
using ApproachTheForge.Spawners;
using Godot;

namespace ApproachTheForge.Utility;

public partial class GameManager : Node2D
{
	[Export] private float _spawnRate = 1f;
	public ResourceManager ResourceManager { get; private set; }
	public UpgradeManager UpgradeManager { get; private set; }
	public AbilityController AbilityController { get; } = new();
	public PlacementController PlacementController { get; private set; }
	
	public Player Player { get; private set; }

	private EnemyGolemSpawner _enemyGolemSpawner;
	private PlayerSpawner _playerSpawner;
	private int _wave;
	private float _timeSinceLastSpawn;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ResourceManager = GetNode<ResourceManager>("ResourceManager");
		PlacementController = GetNode<PlacementController>("PlacementController");
		UpgradeManager = GetNode<UpgradeManager>("UpgradeManager");

		ResourceManager.Initialize(this);
		PlacementController.Initialize(this);
		
		_enemyGolemSpawner = GetNode<EnemyGolemSpawner>("EnemyGolemSpawner");
		_playerSpawner = GetNode<PlayerSpawner>("PlayerSpawner");

		Player = _playerSpawner.Spawn();
		_enemyGolemSpawner.Spawn();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		AbilityController.HandleInput();
		
		if (_timeSinceLastSpawn >= _spawnRate)
		{
			_timeSinceLastSpawn = 0;
			_enemyGolemSpawner.Spawn();
		}

		_timeSinceLastSpawn += (float)delta;
	}
}
