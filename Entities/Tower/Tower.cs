using System;
using ApproachTheForge.Utility;
using Godot;

namespace ApproachTheForge.Entities.Tower;

public partial class Tower : Node2D, IPlaceable
{
	[Export] public ResourceType ResourceType { get; set; }
	[Export] public int ResourceConsumptionAmount { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}