using System;
using Godot;

namespace ApproachTheForge.Utility;

public interface IPlaceable
{
    public static event Action OnPlacement;
	
    public bool Visible { get; set; }
	
    public Vector2 GlobalPosition { get; set; }
    
    public Color Modulate { get; set; }
    
    public Type PlaceableType { get; set; }
    
    public ResourceType ResourceType { get; set; }
    
    public int ResourceConsumptionAmount { get; set; }
}