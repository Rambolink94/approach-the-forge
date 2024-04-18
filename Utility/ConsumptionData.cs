using Godot;

namespace ApproachTheForge.Utility;

[GlobalClass]
public partial class ConsumptionData : Resource
{
    [Export] public ResourceType ResourceType { get; set; }
    [Export] public int ResourceConsumptionAmount { get; set; }
}