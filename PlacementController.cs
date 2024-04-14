using System.Collections.Generic;
using System.Diagnostics;
using Godot;

namespace ApproachTheForge;

public partial class PlacementController : Node2D
{
	[Export] private int _snapIncrement = 64;
	[Export] private bool _useSnapping = true;
	[Export] private bool _showDebugInfo = true;
	
	private PackedScene _tower = GD.Load<PackedScene>("res://Actors/tower.tscn");

	private Dictionary<string, IPlaceable> _inputMap;
	private Dictionary<IPlaceable, PackedScene> _instanceMap;
	private IPlaceable _activePlaceable;
	private bool _placementActive;
	private bool _validPlacement;
	private bool _mouseButtonConsumed = true;
	private RayCast2D _placementRay;
	private Camera2D _camera;
	private Node2D _placeableParent;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_placeableParent = GetNode<Node2D>("PlaceableParent");
		_placementRay = GetNode<RayCast2D>("PlacementRay");
		_camera = GetViewport().GetCamera2D();

		var tower = CreatePlaceable<IPlaceable>(GlobalPosition, _tower, false, this);

		_inputMap = new Dictionary<string, IPlaceable>
		{
			{ "player_tower_select", tower },
		};

		_instanceMap = new Dictionary<IPlaceable, PackedScene>
		{
			{ tower, _tower },
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		ProcessInput();
		DebugDraw();

		if (!_placementActive) return;
		
		Vector2 mousePos = _camera.GetGlobalMousePosition();
		
		_placementRay.GlobalPosition = mousePos + Vector2.Up * GetViewport().GetVisibleRect().Size.Y;
		
		if (_placementRay.IsColliding())
		{
			Vector2 normal = _placementRay.GetCollisionNormal();
			if (normal.Dot(Vector2.Up) >= 1)
			{
				_validPlacement = true;
				Vector2 point = _placementRay.GetCollisionPoint();
				if (_useSnapping)
				{
					/*
					TODO: Revisit this.
					var spaceState = GetWorld2D().DirectSpaceState;
					var queryOffsetPos = point + Vector2.Up * _snapIncrement / 2;
					var raycastRight = PhysicsRayQueryParameters2D.Create(queryOffsetPos, Vector2.Right * _snapIncrement);
					var raycastLeft = PhysicsRayQueryParameters2D.Create(queryOffsetPos, Vector2.Left * _snapIncrement);

					var leftIntersections = spaceState.IntersectRay(raycastLeft);
					var rightIntersections = spaceState.IntersectRay(raycastRight);
					if (leftIntersections.Count > 0
					    || rightIntersections.Count > 0)
					{
						_validPlacement = false;
					}
					*/
					
					point.X = Mathf.Round(point.X / _snapIncrement) * _snapIncrement;
				}
				
				_activePlaceable.GlobalPosition = point;
			}
			else
			{
				GD.Print("INVALID");
				_validPlacement = false;
			}
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left } mouseEvent
		    && _placementActive
		    && _validPlacement)
		{
			if (mouseEvent.Pressed && _mouseButtonConsumed)
			{
				_mouseButtonConsumed = false;
				if (_instanceMap.TryGetValue(_activePlaceable, out PackedScene package))
				{
					CreatePlaceable<IPlaceable>(_activePlaceable.GlobalPosition, package, true);
				}
			}
			else if (!mouseEvent.Pressed)
			{
				_mouseButtonConsumed = true;
			}
		}
	}

	public override void _Draw()
	{
		if (_activePlaceable is null) return;
		
		var position = _activePlaceable.GlobalPosition;
		DrawLine(
			position + Vector2.Up * GetViewport().GetVisibleRect().Size.Y,
			position, Colors.Aqua, 4f);

		var offsetPos = position + Vector2.Up * _snapIncrement / 2;
		DrawLine(offsetPos, offsetPos + Vector2.Right * _snapIncrement, Colors.Red, 4f);
		DrawLine(offsetPos, offsetPos + Vector2.Left * _snapIncrement, Colors.Blue, 4f);
	}

	[Conditional("DEBUG")]
	private void DebugDraw()
	{
		if (_showDebugInfo)
		{
			QueueRedraw();
		}
	}

	private T CreatePlaceable<T>(Vector2 position, PackedScene package, bool isVisible, Node2D parent = null)
		where T : class, IPlaceable
	{
		var placeable = package.Instantiate<T>();
		placeable.Visible = isVisible;
		placeable.GlobalPosition = position;

		parent ??= _placeableParent;
		parent.AddChild(placeable as Node2D);
		return placeable;
	}

	private void ProcessInput()
	{
		foreach (var pair in _inputMap)
		{
			if (!Input.IsActionJustPressed(pair.Key)) continue;
			
			if (pair.Value == _activePlaceable)
			{
				_placementActive = false;
				if (_activePlaceable is not null)
				{
					_activePlaceable.Visible = false;
				}

				_activePlaceable = null;
				continue;
			}

			_placementActive = true;
			_activePlaceable = pair.Value;
			_activePlaceable.Visible = true;

			return;
		}
	}
}

public interface IPlaceable
{
	public bool Visible { get; set; }
	
	public Vector2 GlobalPosition { get; set; }
}