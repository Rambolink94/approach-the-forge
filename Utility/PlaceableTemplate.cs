using Godot;

namespace ApproachTheForge.Utility;

public partial class PlaceableTemplate : Node2D, IPlaceable
{
    private Sprite2D _sprite;

    public Sprite2D Sprite => _sprite ?? GetNode<Sprite2D>("Sprite2D");
}