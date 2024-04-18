using System.Collections.Generic;
using ApproachTheForge.Entities;
using ApproachTheForge.Utility;
using Godot;

namespace ApproachTheForge.Spawners;

public abstract partial class Spawner<T> : Node2D
	where T : Entity
{
	protected abstract PackedScene EntityScene { get; }

	private GameManager _gameManager;

	public override void _Ready()
	{
		Node root = GetTree().Root.GetNode("Game");
		_gameManager = root.GetNode<GameManager>("GameManager");
	}

	public virtual T Spawn(Vector2 position = default)
	{
		var entity = EntityScene.Instantiate<T>();
		entity.Initialize(_gameManager);
		
		AddChild(entity);

		return entity;
	}
}
