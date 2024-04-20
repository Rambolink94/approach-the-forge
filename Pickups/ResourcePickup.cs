using System;
using ApproachTheForge.Utility;
using Godot;

namespace ApproachTheForge.Pickups;

public partial class ResourcePickup : StaticBody2D
{
	public event Action<ResourcePickup> OnCollected;
	
	[Export] public ResourceType ResourceType { get; private set; }

	public void Collect()
	{
		OnCollected?.Invoke(this);
	}
}