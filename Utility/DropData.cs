using Godot;

namespace ApproachTheForge.Utility;

[GlobalClass]
public partial class DropData : ResourceData
{
	[Export(PropertyHint.Range, "0.0, 1.0")] public float DropChance { get; set; }
}