using System.Collections.Generic;
using System.Linq;
using ApproachTheForge.Utility;
using ApproachTheForge.Utility.Extensions;
using Godot;

namespace ApproachTheForge.UI.GameplayHUD;

public partial class AbilityUIController : CanvasLayer
{
	[Export] private TextureRect _selectionRect;
	
	private readonly List<MarginContainer> _containers = new();
	private int _currentIndex;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var parentButton = _selectionRect.GetParent<MarginContainer>();
		int index = 0;
		foreach (var node in GetChild(0).GetChildren())
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
}