using ApproachTheForge.Entities.Golem;
using ApproachTheForge.Spawners;
using Godot;

namespace ApproachTheForge;

public partial class GolemSpawner : Spawner<EnemyGolemAI>
{
	// Called when the node enters the scene tree for the first time.
	protected override PackedScene EntityScene => GD.Load<PackedScene>("res://Entities/Golem/hostile_golem.tscn");
}