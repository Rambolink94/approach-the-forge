using Godot;

namespace ApproachTheForge.Entities.Tower;

public partial class Tower : Node2D, IPlaceable
{
	private Sprite2D _sprite;

	public Sprite2D Sprite => _sprite ?? GetNode<Sprite2D>("Sprite2D");
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}