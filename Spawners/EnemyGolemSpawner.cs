using ApproachTheForge.Entities.Golem;
using ApproachTheForge.Entities.Player;
using ApproachTheForge.Utility;
using Godot;

namespace ApproachTheForge.Spawners;

public partial class EnemyGolemSpawner : Node2D, ISpawner<EnemyGolemAI>
{
	// Called when the node enters the scene tree for the first time.
	public PackedScene EntityScene => GD.Load<PackedScene>("res://Entities/Golem/hostile_golem.tscn");
	private GameManager _gameManager;

	public override void _Ready()
	{
		Node root = GetTree().Root.GetNode("Game");
		_gameManager = root.GetNode<GameManager>("GameManager");
	}
	
	public EnemyGolemAI Spawn(Vector2 position = default)
	{
		var entity = EntityScene.Instantiate<EnemyGolemAI>();
		entity.Initialize(_gameManager);
		
		AddChild(entity);

		return entity;
	}
}
