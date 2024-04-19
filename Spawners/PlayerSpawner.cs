using ApproachTheForge.Entities.Player;
using ApproachTheForge.Utility;
using Godot;

namespace ApproachTheForge.Spawners;

public partial class PlayerSpawner : Node2D, ISpawner<Player>
{
	public PackedScene EntityScene => GD.Load<PackedScene>("res://Entities/Player/player.tscn");
	
	private GameManager _gameManager;

	public override void _Ready()
	{
		Node root = GetTree().Root.GetNode("Game");
		_gameManager = root.GetNode<GameManager>("GameManager");
	}
	
	public Player Spawn(Vector2 position = default)
	{
		var entity = EntityScene.Instantiate<Player>();
		entity.Initialize(_gameManager);
		
		AddChild(entity);

		return entity;
	}
}
