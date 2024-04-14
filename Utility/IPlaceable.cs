using System;
using Godot;

namespace ApproachTheForge.Utility;

public interface IPlaceable
{
    public static event Action OnPlacement;
	
    public bool Visible { get; set; }
	
    public Vector2 GlobalPosition { get; set; }
	
    public Sprite2D Sprite { get; }
}