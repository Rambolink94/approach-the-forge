using Godot;

namespace ApproachTheForge.Utility;

public partial class PlacementTemplate : Node2D
{
    [Export] public Resource ConsumptionData { get; set; }
    [Export] public PackedScene Placeable { get; private set; }
    [Export] public bool IsTower { get; private set; }
}