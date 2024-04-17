using System;
using System.Collections.Generic;
using Godot;

namespace ApproachTheForge.Utility;

public class AbilityController
{
    public delegate void AbilityChangedEventHandler(string action, int index);
    public static event AbilityChangedEventHandler AbilityChanged;

    private readonly Dictionary<string, int> _actionMap = new()
    {
        { "player_stealth_sprint", 0 },
        { "player_tower_select", 1 },
        { "player_golem_select", 2 },
    };

    public void HandleInput()
    {
        foreach (var action in _actionMap)
        {
            if (Input.IsActionJustPressed(action.Key))
            {
                AbilityChanged?.Invoke(action.Key, action.Value);
            }
        }
    }
}