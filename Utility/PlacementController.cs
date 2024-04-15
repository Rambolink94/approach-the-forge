using System;
using System.Collections.Generic;
using System.Diagnostics;
using ApproachTheForge.Entities.Golem;
using ApproachTheForge.Entities.Tower;
using Godot;

namespace ApproachTheForge.Utility;

public partial class PlacementController : Node2D
{
	[Export] private int _unitSize = 64;
	[Export] private Color _invalidColor = Colors.Red;
	[Export] private Color _validColor = Colors.Green;
	[Export] private bool _showDebugInfo = true;
	
	private PackedScene _tower = GD.Load<PackedScene>("res://Entities/Tower/tower.tscn");
	private PackedScene _golem = GD.Load<PackedScene>("res://Entities/Golem/Friendly_Golem.tscn");

	private Dictionary<string, IPlaceable> _inputMap;
	private Dictionary<IPlaceable, PackedScene> _instanceMap;
	private IPlaceable _activePlaceable;
	private bool _placementActive;
	private bool _validPlacement;
	private bool _mouseButtonConsumed = true;
	
	private RayCast2D _placementRay;
	private RayCast2D _validityRay;
	private Camera2D _camera;
	private Node2D _placeableParent;
	private AudioStreamPlayer2D _audioStream;
	private GameManager _gameManager;
	
	// Debug
	private Vector2 _leftInvalidPosition;
	private Vector2 _rightInvalidPosition;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_placeableParent = GetNode<Node2D>("PlaceableParent");
		_placementRay = GetNode<RayCast2D>("PlacementRay");
		_validityRay = GetNode<RayCast2D>("ValidityRay");
		_audioStream = GetNode<AudioStreamPlayer2D>("PlacementPlayer");
		_gameManager = GetNode<GameManager>("../GameManager");

		var tower = CreatePlaceable<IPlaceable>(GlobalPosition, _tower, false, this);
		var golem = CreatePlaceable<IPlaceable>(GlobalPosition, _golem, false, this);

		_inputMap = new Dictionary<string, IPlaceable>
		{
			{ "player_tower_select", tower },
			{ "player_golem_select", golem },
		};

		_instanceMap = new Dictionary<IPlaceable, PackedScene>
		{
			{ tower, _tower },
			{ golem, _golem },
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		ProcessInput();
		DebugDraw();

		if (!_placementActive) return;

		_camera ??= GetViewport().GetCamera2D();
		Vector2 mousePos = _camera.GetGlobalMousePosition();

		Vector2 rayOffset = mousePos + Vector2.Up * GetViewport().GetVisibleRect().Size.Y;
		_placementRay.GlobalPosition = rayOffset;
		_validityRay.GlobalPosition = rayOffset;
		
		if (_placementRay.IsColliding())
		{
			Vector2 normal = _placementRay.GetCollisionNormal();
			Vector2 point = _placementRay.GetCollisionPoint();
			
			// This is gross...but it works
			bool isValid = normal.Dot(Vector2.Up) >= 1
			               && (!_validityRay.IsColliding()
			                   || _validityRay.GetCollider() is Node2D collider
			                   && collider.GetParentOrNull<Tower>() is not null
			                   && _activePlaceable is FriendlyGolemAI);
			
			MarkValidity(isValid);
			
			_activePlaceable.GlobalPosition = point;
		}
	}

	public override void _Draw()
	{
		if (_activePlaceable is null) return;
		
		var position = _activePlaceable.GlobalPosition;
		DrawLine(
			position + Vector2.Up * GetViewport().GetVisibleRect().Size.Y,
			position, Colors.Aqua, 4f);
	}

	[Conditional("DEBUG")]
	private void DebugDraw()
	{
		if (_showDebugInfo)
		{
			QueueRedraw();
		}
	}

	private void MarkValidity(bool isValid)
	{
		_validPlacement = isValid;
		_activePlaceable.Modulate = isValid ? _validColor : _invalidColor;
	}

	private T CreatePlaceable<T>(Vector2 position, PackedScene package, bool isVisible, Node2D parent = null, bool isReal = false)
		where T : class, IPlaceable
	{
		var placeable = package.Instantiate<T>();
		placeable.Visible = isVisible;
		placeable.GlobalPosition = position;
		
		parent ??= _placeableParent;
		if (placeable is Node2D node)
		{
			parent.AddChild(node);
		}

		if (isReal)
		{
			_audioStream.Play();
		}
		else
		{
			placeable.SetAsPlacementTemplate();
		}

		return placeable;
	}

	private void ProcessInput()
	{
		if (Input.IsActionJustPressed("player_action"))
		{
			if (_validPlacement
			    && _instanceMap.TryGetValue(_activePlaceable, out PackedScene package)
			    && _gameManager.ResourceManager.TryUseResource(_activePlaceable.ResourceType, _activePlaceable.ResourceConsumptionAmount))
			{
				CreatePlaceable<IPlaceable>(_activePlaceable.GlobalPosition, package, true, isReal: true);
			}
		}
		
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
			if (_activePlaceable is not null)
			{
				_activePlaceable.Visible = false;
			}
			
			_activePlaceable = pair.Value;
			_activePlaceable.Visible = true;

			return;
		}
	}
}