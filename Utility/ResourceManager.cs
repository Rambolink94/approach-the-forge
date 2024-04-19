using System;
using ApproachTheForge.Pickups;
using Godot;
using Godot.Collections;

namespace ApproachTheForge.Utility;

public partial class ResourceManager : Node2D, IGameSystem
{
	[ExportCategory("Initial Values")]
	[Export] private int _initialCommon;
	[Export] private int _initialRare;
	[Export] private int _initialSuper;
	
	[ExportCategory("Resource Instance Data")]
	[Export] private Dictionary<ResourceType, PackedScene> _resourceInstanceMap;
	
	public delegate void ResourceChangedEventHandler(ResourceType resourceType, int newCount);
	public static event ResourceChangedEventHandler ResourceChanged;

	private System.Collections.Generic.Dictionary<ResourceType, int> _resourceMap;
    
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_resourceMap = new System.Collections.Generic.Dictionary<ResourceType, int>()
		{
			{ ResourceType.Common, _initialCommon },
			{ ResourceType.Rare, _initialRare },
			{ ResourceType.Super, _initialSuper },
		};

		CallDeferred(nameof(HookEvents));
	}
	
	public void Initialize(GameManager gameManager)
	{
		CallDeferred(nameof(HookEvents));
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

	public bool TryUseResource(ResourceData resourceData)
	{
		return TryUseResource(resourceData.ResourceType, resourceData.ResourceAmount);
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

	public void AddResource(ResourcePickup pickup)
	{
		AddResource(pickup.ResourceType);
	}
	
	public ResourcePickup CreatePickupInstance(ResourceType resourceType, Vector2 globalPosition, bool addChild = true)
	{
		if (_resourceInstanceMap is null || _resourceInstanceMap.Count == 0)
		{
			throw new NullReferenceException("The resource map has not been configured in the editor");
		}
		
		if (_resourceInstanceMap.TryGetValue(resourceType, out PackedScene scene))
		{
			if (scene is null)
			{
				return null;
			}
			
			var instance = scene.Instantiate<ResourcePickup>();
			instance.OnCollected += OnResourceCollected;
			instance.GlobalPosition = globalPosition;

			if (addChild)
			{
				AddChild(instance);
			}

			return instance;
		}

		throw new InvalidOperationException($"The resource type {resourceType} was not present in the instance map");
	}

	private void OnResourceCollected(ResourcePickup resource)
	{
		resource.OnCollected -= OnResourceCollected;
		AddResource(resource);
		
		resource.QueueFree();
	}

	private void HookEvents()
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
