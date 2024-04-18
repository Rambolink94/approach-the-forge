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
	
	// Load templates
	private PackedScene _tower = GD.Load<PackedScene>("res://Entities/Tower/tower_placement_template.tscn");
	private PackedScene _golem = GD.Load<PackedScene>("res://Entities/Golem/Friendly_Golem_Placement_Template.tscn");
	
	private RayCast2D _placementRay;
	private RayCast2D _validityRay;
	private Camera2D _camera;
	private Node2D _placeableParent;
	private AudioStreamPlayer2D _audioStream;
	private GameManager _gameManager;
	
	private Dictionary<string, PlacementTemplate> _inputMap;
	private PlacementTemplate _activeTemplate;
	private bool _placementActive;
	private bool _validPlacement;
	
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

		PlacementTemplate towerTemplate = CreateTemplate(_tower);
		PlacementTemplate golemTemplate = CreateTemplate(_golem);

		_inputMap = new Dictionary<string, PlacementTemplate>
		{
			{ "player_tower_select", towerTemplate },
			{ "player_golem_select", golemTemplate },
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
			var isValid = normal.Dot(Vector2.Up) >= 1
			               && (!_validityRay.IsColliding()
			                   || _validityRay.GetCollider() is Node2D collider
			                   && collider.GetParentOrNull<Tower>() is not null
			                   && !_activeTemplate.IsTower);
			
			MarkValidity(isValid);
			
			_activeTemplate.GlobalPosition = point;
		}
	}

	public override void _Draw()
	{
		if (_activeTemplate is null) return;
		
		var position = _activeTemplate.GlobalPosition;
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
		_activeTemplate.Modulate = isValid ? _validColor : _invalidColor;
	}

	private PlacementTemplate CreateTemplate(PackedScene package)
	{
		var towerTemplate = package.Instantiate<PlacementTemplate>();
		towerTemplate.Visible = false;
		
		AddChild(towerTemplate);
		
		return towerTemplate;
	}
	
	private void CreatePlaceable(Vector2 position, PackedScene package)
	{
		var placeable = package.Instantiate<IPlaceable>();
		placeable.GlobalPosition = position;
		
		_placeableParent.AddChild(placeable as Node2D);
		_audioStream.Play();
	}

	private void ProcessInput()
	{
		if (Input.IsActionJustPressed("player_action"))
		{
			if (_validPlacement
			    && _gameManager.ResourceManager.TryUseResource((ConsumptionData)_activeTemplate.ConsumptionData))
			{
				CreatePlaceable(_activeTemplate.GlobalPosition, _activeTemplate.Placeable);
			}
		}
		
		foreach (var pair in _inputMap)
		{
			if (!Input.IsActionJustPressed(pair.Key)) continue;
			
			if (pair.Value == _activeTemplate)
			{
				_placementActive = false;
				if (_activeTemplate is not null)
				{
					_activeTemplate.Visible = false;
				}
				
				_activeTemplate = null;
				continue;
			}

			_placementActive = true;
			if (_activeTemplate is not null)
			{
				_activeTemplate.Visible = false;
			}
			
			_activeTemplate = pair.Value;
			_activeTemplate.Visible = true;

			return;
		}
	}
}