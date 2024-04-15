using System;
using Godot;

namespace ApproachTheForge.Utility;

public interface IPlaceable
{
    public static event Action OnPlacement;
	
    public bool Visible { get; set; }
	
    public Vector2 GlobalPosition { get; set; }
    
    public Color Modulate { get; set; }
    
    public ResourceType ResourceType { get; set; }
    
    public int ResourceConsumptionAmount { get; set; }

    /// <summary>
    ///		Disables default functionality to turn the object into a placeable compatible instance.
    /// </summary>
    public void SetAsPlacementTemplate();
}