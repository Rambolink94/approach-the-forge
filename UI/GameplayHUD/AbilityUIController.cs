using System.Collections.Generic;
using System.Linq;
using ApproachTheForge.Utility;
using Godot;

namespace ApproachTheForge.UI.GameplayHUD;

public partial class AbilityUIController : CanvasLayer
{
	[Export] private TextureRect _selectionRect;
	private List<TextureButton> _buttons = new();

	private int _currentIndex = -1;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (var node in GetChild(0).GetChildren())
		{
			int index = 0;
			if (node is TextureButton button)
			{
				button.Pressed += () => OnAbilityChanged(string.Empty, index++);
				_buttons.Add(button);
			}
		}

		OnAbilityChanged(string.Empty, _currentIndex);
		AbilityController.AbilityChanged += OnAbilityChanged;
	}

	public void OnAbilityChanged(string action, int index)
	{
		if (_currentIndex == -1 || (_selectionRect.Visible && _currentIndex == index))
		{
			_selectionRect.Visible = false;
			return;
		}
		
		_buttons[_currentIndex].RemoveChild(_selectionRect);
		_currentIndex = index;
		_buttons[_currentIndex].AddChild(_selectionRect);
	}
}