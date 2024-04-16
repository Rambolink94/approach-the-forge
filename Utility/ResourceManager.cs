using System;
using System.Collections.Generic;
using Godot;

namespace ApproachTheForge.Utility;

public partial class ResourceManager : Node2D
{
	[Export] private int _initialCommon;
	[Export] private int _initialRare;
	[Export] private int _initialSuper;
	
	public delegate void ResourceChangedEventHandler(ResourceType resourceType, int newCount);
	public static event ResourceChangedEventHandler ResourceChanged;

	private Godot.Collections.Dictionary<ResourceType, int> _resourceMap;
    
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_resourceMap = new Godot.Collections.Dictionary<ResourceType, int>()
		{
			{ ResourceType.Common, _initialCommon },
			{ ResourceType.Rare, _initialRare },
			{ ResourceType.Super, _initialSuper },
		};
		
		CallDeferred(nameof(Init));
	}

	public bool TryUseResource(ResourceType resourceType, int count)
	{
		if (_resourceMap.TryGetValue(resourceType, out var current) && current >= count)
		{
			var newValue = current - count;
			_resourceMap[resourceType] = newValue;
			ResourceChanged?.Invoke(resourceType, newValue);
			
			return true;
		}

		return false;
	}

	public bool TryUseResource(ConsumptionData consumptionData)
	{
		return TryUseResource(consumptionData.ResourceType, consumptionData.ResourceConsumptionAmount);
	}

	public void AddResource(ResourceType resourceType, int count = 1)
	{
		if (_resourceMap.TryGetValue(resourceType, out var current))
		{
			var newCount = current + count;
			_resourceMap[resourceType] = newCount;
			ResourceChanged?.Invoke(resourceType, newCount);
			return;
		}

		throw new InvalidOperationException($"Resource type of {resourceType} is not valid");
	}

	private void Init()
	{
		ResourceChanged?.Invoke(ResourceType.Common, _initialCommon);
		ResourceChanged?.Invoke(ResourceType.Rare, _initialRare);
		ResourceChanged?.Invoke(ResourceType.Super, _initialSuper);
	}
}

public enum ResourceType
{
	None,
	Common,
	Rare,
	Super
}