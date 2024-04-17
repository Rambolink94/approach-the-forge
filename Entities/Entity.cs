using System;
using ApproachTheForge.Utility;
using Godot;
using Godot.Collections;

namespace ApproachTheForge.Entities;

public abstract partial class Entity : CharacterBody2D
{
    [Export] private Array<DropData> _dropTable;

    public GameManager GameManager { get; private set; }
    
    private bool _dropTableValid;

    public override void _Ready()
    {
        // validate drop table
        if (_dropTable == null || _dropTable.Count == 0) return;
        
        float total = 0f;
        foreach (var resource in _dropTable)
        {
            var dropData = (DropData)resource;
            total += dropData.DropChance;
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

    protected virtual void Die()
    {
        if (IsInstanceValid(this))
        {
            QueueFree();
        }
        
        HandleDrops();
    }

    protected virtual void HandleDrops()
    {
        if (!_dropTableValid) return;
        
        // TODO: Implement drop logic
    }
}