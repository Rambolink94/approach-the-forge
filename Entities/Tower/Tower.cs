using System;
using ApproachTheForge.Utility;
using Godot;

namespace ApproachTheForge.Entities.Tower;

public partial class Tower : Entity, IPlaceable, IDamageable
{
	[Export] public float Health { get; private set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public bool ApplyDamage(DamageData damageData)
	{
		Health -= damageData.Damage;
		if (Health <= 0)
		{
			Die();
			return true;
		}

		return false;
	}
}