using System;
using System.Linq;
using ApproachTheForge.Pickups;
using ApproachTheForge.Utility;
using Godot;
using Godot.Collections;

namespace ApproachTheForge.Entities;

public abstract partial class Entity : CharacterBody2D
{
    [Export] private Array<DropData> _dropTable;

    protected GameManager GameManager { get; private set; }
    
    private bool _dropTableValid;

    public override void _Ready()
    {
        // validate drop table
        if (_dropTable == null || _dropTable.Count == 0) return;
        
        float total = 0f;
        foreach (DropData resource in _dropTable)
        {
            total += resource.DropChance;
        }

        if (total is > 1.0f or < 1.0f)
        {
            throw new InvalidOperationException($"DropTable on {GetType()} does not add to 1f");
        }
        
        _dropTableValid = true;
    }

    public void Initialize(GameManager gameManager)
    {
        GameManager = gameManager;
    }

    protected virtual void Die(bool removeOnDeath = true)
    {
        if (IsInstanceValid(this) && removeOnDeath)
        {
            QueueFree();
        }
        
        HandleDrops();
    }

    protected virtual void HandleDrops()
    {
        if (!_dropTableValid) return;

        float role = GD.Randf();

        var orderedData = _dropTable.OrderByDescending(x => x.DropChance).ToList();
        
        DropData validDropData = orderedData[0];
        for (int i = 1; i < orderedData.Count; i++)
        {
            var dropData = orderedData[i];
            if (dropData.DropChance >= role)
            {
                validDropData = dropData;
            }
        }

        ResourcePickup pickup =
            GameManager.ResourceManager.CreatePickupInstance(validDropData.ResourceType, GlobalPosition);
    }
}