using System;
using System.Collections.Generic;
using ApproachTheForge.Utility;
using ApproachTheForge.Utility.Extensions;
using Godot;

namespace ApproachTheForge.UI.GameplayHUD;

public partial class AbilityUIController : CanvasLayer
{
	private readonly List<MarginContainer> _containers = new();
	private TextureRect _selectionRect;
	private ProgressBar _healthBar;
	private GameManager _gameManager;
	private int _currentIndex;
	private bool _healthBarInitialized;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_healthBar = GetNode<ProgressBar>("VBoxContainer/Health/HBoxContainer/HealthBar");
		_gameManager = GetTree().Root.GetNode("Game").GetNode<GameManager>("GameManager");
		_gameManager.Player.HealthChanged += UpdateHealth;
		UpdateHealth(_gameManager.Player.Health);

		_selectionRect = 
			GetNode<TextureRect>("VBoxContainer/Abilities/HBoxContainer/DashButton/MarginContainer/SelectionRect");
		var parentButton = _selectionRect.GetParent<MarginContainer>();
		if (parentButton is null)
		{
			throw new NullReferenceException("The provided selection rect was not parented...somehow");
		}
		
		int index = 0;
		var buttonRoot = GetNode("VBoxContainer/Abilities/HBoxContainer");
		foreach (var node in buttonRoot.GetChildren())
		{
			if (node is TextureButton button)
			{
				var localIndex = index++;
				button.Pressed += () => OnAbilityChanged(string.Empty, localIndex);
				var container = button.GetTypeFromChildren<MarginContainer>();
				_containers.Add(container);
			}
		}
		
		_currentIndex = _containers.IndexOf(parentButton);
		OnAbilityChanged(string.Empty, -1);
		AbilityController.AbilityChanged += OnAbilityChanged;
	}

	public void OnAbilityChanged(string action, int index)
	{
		// TODO: Clean this up. Doing it with indexes and strings is subject to fail
		// Consider adding ability data object to Ability controller
		if (index == -1 || (_selectionRect.Visible && _currentIndex == index))
		{
			_selectionRect.Visible = false;
			return;
		}
		
		_containers[_currentIndex].RemoveChild(_selectionRect);
		_currentIndex = index;
		_containers[_currentIndex].AddChild(_selectionRect);
		_selectionRect.Visible = true;
	}

	private void UpdateHealth(float health)
	{
		if (!_healthBarInitialized)
		{
			_healthBar.MaxValue = health;
			_healthBarInitialized = true;
		}
		
		_healthBar.Value = health;
	}
}