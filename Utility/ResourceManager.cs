using Godot;

namespace ApproachTheForge.Utility;

public partial class ResourceManager : Node2D
{
	private int _common;
	private int _rare;
	private int _super;

	[Export]
	public int Common
	{
		get => _common;
		set => _common = SetResourceCount(ResourceType.Common, _common, value);
	}

	[Export]
	public int Rare
	{
		get => _rare;
		set => _rare = SetResourceCount(ResourceType.Rare, _rare, value);
	}

	[Export]
	public int Super
	{
		get => _super;
		set => _super = SetResourceCount(ResourceType.Super, _super, value);
	}
	
	public delegate void ResourceChangedEventHandler(ResourceType resourceType, int newCount);
	public static event ResourceChangedEventHandler ResourceChanged;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CallDeferred(nameof(Init));
	}

	private void Init()
	{
		ResourceChanged?.Invoke(ResourceType.Common, Common);
		ResourceChanged?.Invoke(ResourceType.Rare, Rare);
		ResourceChanged?.Invoke(ResourceType.Super, Super);
	}

	private static int SetResourceCount(ResourceType resourceType, int currentValue, int newValue)
	{
		if (currentValue != newValue)
		{
			ResourceChanged?.Invoke(resourceType, newValue);
		}

		return newValue;
	}
}

public enum ResourceType
{
	Common,
	Rare,
	Super
}
