using System.Collections.Generic;
using System.Linq;
using ApproachTheForge.Utility;
using ApproachTheForge.Utility.Upgrade;
using Godot;

namespace ApproachTheForge.Entities.Golem
{
	public partial class FriendlyGolemAI : GolemAI, IPlaceable
	{
		[Export] public ResourceType ResourceType { get; set; }
		[Export] public int ResourceConsumptionAmount { get; set; }
		protected override Bearing ObjectiveBearing => Bearing.Right;

		protected override double MaxHealth => this.CalculateUpgradedValue(this.BaseHealth, EntityStatistics.Health);

		protected override double Health { get; set; }

		protected override double Damage => this.CalculateUpgradedValue(this.BaseDamage, EntityStatistics.Damage);

		protected override DamageData DamageToApply => new()
		{
			Damage = (float)this.Damage,
			Knockback = new Vector2((int)this.Bearing * 300, 0),
		};

		public override void _Ready()
		{
			base._Ready();

			this.Speed = (float)this.CalculateUpgradedValue(this.Speed, EntityStatistics.Speed);
		}

		public override void _PhysicsProcess(double delta)
		{
			base._PhysicsProcess(delta);

			if (SearchForEnemies())
			{
				this.FaceTarget();
			}
			else
			{
				this.FaceObjective();
			}
		}

		public override void _Process(double delta)
		{
			base._Process(delta);

			if (SearchForEnemies())
			{
				this.AttackTargetInRange();
			}
			else
			{
				this.FaceObjective();
			}
		}

		public bool SearchForEnemies()
		{
			if (this.DetectionArea.HasOverlappingBodies())
			{
				this.Target = this.DetectionArea.GetOverlappingBodies().FirstOrDefault();

				return true;
			}
			else
			{
				return false;
			}
		}

		public double CalculateUpgradedValue(double health, EntityStatistics stat)
		{
			IEnumerable<GolemUpgrade> upgrades = this.GameManager.UpgradeManager.GetUpgrades<GolemUpgrade>().
				Where(upgrade => upgrade.UpgradedStat == stat);

			foreach (GolemUpgrade upgrade in upgrades)
			{

				// Increase the stat multiplicatively by the upgrade value.
				health *= 1 + (upgrade.UpgradeValue / 100);
			}

			return health;
		}
	}
}


