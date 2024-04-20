using System;
using System.Collections.Generic;
using Godot;

namespace ApproachTheForge.Utility;

public class AbilityController
{
    public delegate void AbilityChangedEventHandler(string action, int index);
    public static event AbilityChangedEventHandler AbilityChanged;

    private static readonly Dictionary<string, int> ActionMap = new()
    {
        { "player_stealth_sprint", 0 },
        { "player_tower_select", 1 },
        { "player_golem_select", 2 },
    };

    public void HandleInput()
    {
        foreach (var action in ActionMap)
        {
            if (Input.IsActionJustPressed(action.Key))
            {
                AbilityChanged?.Invoke(action.Key, action.Value);
            }
        }
    }

    public static void ChangeAbility(string input)
    {
        if (ActionMap.TryGetValue(input, out int index))
        {
            AbilityChanged?.Invoke(input, index);
        }
    }
}