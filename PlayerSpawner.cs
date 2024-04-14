using ApproachTheForge.Entities.Player;
using ApproachTheForge.Spawners;
using Godot;

namespace ApproachTheForge;

public partial class PlayerSpawner : Spawner<Player>
{
	protected override PackedScene EntityScene => GD.Load<PackedScene>("res://Entities/Player/player.tscn");

	public override void _Ready()
	{
		base._Ready();
		
		Spawn();
	}
}