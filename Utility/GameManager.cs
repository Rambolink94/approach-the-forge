using System;
using ApproachTheForge.Entities.Player;
using ApproachTheForge.Spawners;
using Godot;

namespace ApproachTheForge.Utility;

public partial class GameManager : Node2D
{
	[Export] private int _startWave = 1;
	[Export] private float _initialWaveLength = 60f;
	[Export] private int _initialSpawnCount = 20;
	[Export] private float _initialSpawnRate = 5f;
		
	public ResourceManager ResourceManager { get; private set; }
	public UpgradeManager UpgradeManager { get; private set; }
	public AbilityController AbilityController { get; } = new();
	public PlacementController PlacementController { get; private set; }
	
	public GameOverController GameOverScreen { get; private set; }
	
	public Player Player { get; private set; }

	public delegate void WaveProgressEventHandler(float timeRemaining, int killsInWave);

	public delegate void WaveChangedEventHandler(int wave, float waveLength, int spawnCount);

	public event WaveProgressEventHandler WaveProgress;
	public event WaveChangedEventHandler WaveChanged;
	
	private EnemyGolemSpawner _enemyGolemSpawner;
	private PlayerSpawner _playerSpawner;
	private float _timeSinceLastSpawn;
	
	private int _wave;
	private float _waveLength;
	private int _waveSpawnCount;
	private int _currentWaveSpawnCount;
	private float _spawnRate;
	private int _waveKillCount;
	private int _totalKillCount;
	private float _timeSinceLastWave;
	private double _gameTime;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ResourceManager = GetNode<ResourceManager>("ResourceManager");
		PlacementController = GetNode<PlacementController>("PlacementController");
		UpgradeManager = GetNode<UpgradeManager>("UpgradeManager");
		GameOverScreen = GetNode<GameOverController>("GameOverScreen");


		ResourceManager.Initialize(this);
		PlacementController.Initialize(this);
		
		_enemyGolemSpawner = GetNode<EnemyGolemSpawner>("EnemyGolemSpawner");
		_playerSpawner = GetNode<PlayerSpawner>("PlayerSpawner");

		Player = _playerSpawner.Spawn();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_gameTime += delta;
		AbilityController.HandleInput();
		
		HandleSpawn((float)delta);
	}

	public void HandleSpawn(float delta)
	{
		_timeSinceLastWave += delta;
		if (_timeSinceLastWave >= _waveLength)
		{
			InitializeWave();
			_timeSinceLastWave = 0;
		}
		
		if (_timeSinceLastSpawn >= _spawnRate)
		{
			_timeSinceLastSpawn = 0;
			var golem = _enemyGolemSpawner.Spawn();
			golem.Removed += OnEntityRemoved;
		}

		WaveProgress?.Invoke(_timeSinceLastWave, _waveKillCount);
		_timeSinceLastSpawn += delta;
	}

	private void InitializeWave()
	{
		_wave++;
		
		// TODO: This needs to be further fleshed out
		// For example, enemy golem health should increase.
		// Spawn rates should also be increased.
		_waveLength = (float)(_gameTime / _wave + _initialWaveLength);
		_waveSpawnCount = Mathf.FloorToInt(_gameTime * 1.25f / _wave + _initialSpawnCount);
		_spawnRate = _waveLength / _waveSpawnCount;
		_waveKillCount = 0;
		
		WaveChanged?.Invoke(_wave, _waveLength, _waveSpawnCount);
	}

	private void OnEntityRemoved()
	{
		_totalKillCount++;
		_waveKillCount++;
	}
}
