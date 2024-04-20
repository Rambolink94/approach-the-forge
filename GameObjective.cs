using ApproachTheForge;
using ApproachTheForge.Utility;
using Godot;
using System;

public partial class GameObjective : Node2D, IDamageable
{
	[Export] public float _baseHealth = 100;
	[Export] public bool _isPlayerObjective = true;

	public float Health { get; private set; }

	private GameManager GameManager;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.GameManager = GetTree().Root.GetNode("Game").GetNode<GameManager>("GameManager");

		this.Health = this._baseHealth;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public bool ApplyDamage(DamageData damageInstance)
	{
		this.Health -= damageInstance.Damage;

		GD.Print(this.Name + " has taken " + damageInstance.Damage + " damage");

		if(this.Health < 0)
		{
			if(_isPlayerObjective)
			{
				this.GameManager.GameOverScreen.Open("You Are Victorious");
			}
			else
			{
				this.GameManager.GameOverScreen.Open("Village Destroyed", true);
			}
			return true;
		}

		return false;
	}
}
