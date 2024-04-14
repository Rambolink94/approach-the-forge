using ApproachTheForge.Utility;
using Godot;

namespace ApproachTheForge.Entities;

public abstract partial class Entity : CharacterBody2D
{
    public GameManager GameManager { get; private set; }
    
    public void Initialize(GameManager gameManager)
    {
        GameManager = gameManager;
    }
}