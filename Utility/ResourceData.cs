using Godot;

namespace ApproachTheForge.Utility;

[GlobalClass]
public partial class ResourceData : Resource
{
    [Export] public ResourceType ResourceType { get; set; }
    [Export] public int ResourceAmount { get; set; }
}