using ApproachTheForge.Entities.Tower;
using ApproachTheForge.Utility.Upgrade;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApproachTheForge.Entities.Tower;

public partial class UpgradableTower : Tower
{
	[Export] private double BaseHealth { get; set; } = 100;
	[Export] private double BaseDamage { get; set; } = 30;

	[Export] private double BaseRateOfFire { get; set; } = 1.5;

	private double Health { get; set; }

	private double MaxHealth => this.CalculateUpgradedStat(this.BaseHealth, EntityStatistics.Health);

	private double Damage { get; set; }

	private double RateOfFire { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        this.Health = this.MaxHealth;
		this.Damage = this.CalculateUpgradedStat(this.BaseDamage, EntityStatistics.Damage);
		this.RateOfFire = this.CalculateUpgradedStat(this.BaseRateOfFire, EntityStatistics.Rate_Of_Fire);
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private double CalculateUpgradedStat(double baseStat, EntityStatistics stat)
	{
        IEnumerable<TowerUpgrade> towerUpgrades = this._gameManager.UpgradeManager.
			GetUpgrades<TowerUpgrade>().
			Where(upgrade => upgrade.UpgradedStat == stat);

		foreach(TowerUpgrade upgrade in towerUpgrades)
		{
			baseStat += 1 + (upgrade.UpgradeValue / 100);
		}

		return baseStat;
    }
}
