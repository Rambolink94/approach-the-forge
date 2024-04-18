using ApproachTheForge.Utility;
using Godot;

namespace ApproachTheForge.Pickups;

public partial class ResourcePickup : StaticBody2D
{
	[Export] public ResourceType ResourceType { get; private set; }
}