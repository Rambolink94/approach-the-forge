using Godot;

namespace ApproachTheForge;

public partial class CameraFollow : Node2D
{
	[Export] private float _smoothSpeed = 0.05f;
	[Export] private NodePath _targetPath;
	
	private Camera2D _camera;
	private Node2D _target;
	private Vector2 _velocity;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_camera = GetNode<Camera2D>("Camera2D");
		_target = GetNode<Node2D>(_targetPath);

		GlobalPosition = _target.GlobalPosition;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalPosition = GlobalPosition.Lerp(_target.GlobalPosition, _smoothSpeed);
	}
}
