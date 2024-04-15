using System;
using ApproachTheForge.Utility;
using Godot;

namespace ApproachTheForge.UI.GameplayHUD;

public partial class ResourceDisplayController : CanvasLayer
{
	private Label _commonLabel;
	private Label _rareLabel;
	private Label _epicLabel;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_commonLabel = GetNode<Label>("ResourceIcons/Common/Quantity/Number");
		_rareLabel = GetNode<Label>("ResourceIcons/Rare/Quantity/Number");
		_epicLabel = GetNode<Label>("ResourceIcons/Epic/Quantity/Number");

		ResourceManager.ResourceChanged += OnResourceChanged;
	}

	private void OnResourceChanged(ResourceType resourceType, int newCount)
	{
		Label label = resourceType switch
		{
			ResourceType.Common => _commonLabel,
			ResourceType.Rare => _rareLabel,
			ResourceType.Super => _epicLabel,
			_ => throw new ArgumentException($"Resource of type {resourceType} is invalid", nameof(resourceType)),
		};

		label.Text = newCount.ToString();
	}
}