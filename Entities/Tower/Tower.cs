using System;
using ApproachTheForge.Utility;
using Godot;

namespace ApproachTheForge.Entities.Tower;

public partial class Tower : Node2D, IPlaceable
{
	[Export] public ResourceType ResourceType { get; set; }
	[Export] public int ResourceConsumptionAmount { get; set; }

	private float _delay = 1f;
	private float _currentTime;
	
	public Sprite2D Sprite => _sprite ?? GetNode<Sprite2D>("Sprite2D");

	public Type PlaceableType { get; set; }
	
	private Sprite2D _sprite;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}