using System;
using Godot;

namespace ApproachTheForge.Utility;

public interface IPlaceable
{
    public bool Visible { get; set; }
	
    public Vector2 GlobalPosition { get; set; }
    
    public Color Modulate { get; set; }
}