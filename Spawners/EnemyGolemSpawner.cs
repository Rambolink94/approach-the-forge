using ApproachTheForge.Entities.Golem;
using Godot;

namespace ApproachTheForge.Spawners;

public partial class EnemyGolemSpawner : Spawner<EnemyGolemAI>
{
	// Called when the node enters the scene tree for the first time.
	protected override PackedScene EntityScene => GD.Load<PackedScene>("res://Entities/Golem/hostile_golem.tscn");
}
