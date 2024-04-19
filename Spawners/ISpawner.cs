using ApproachTheForge.Entities;
using Godot;

namespace ApproachTheForge.Spawners;

public interface ISpawner<out T>
    where T : Entity
{
    public PackedScene EntityScene { get; }

    public T Spawn(Vector2 position = default);
}