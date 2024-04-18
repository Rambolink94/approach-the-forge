using System;
using System.Linq;
using ApproachTheForge.Utility;
using Godot;

namespace ApproachTheForge.Entities.Golem;

public partial class FriendlyGolemAI : GolemAI, IPlaceable
{
	[Export] public ResourceType ResourceType { get; set; }
	[Export] public int ResourceConsumptionAmount { get; set; }

	protected override Bearing ObjectiveBearing => Bearing.Right;

		protected override double RateOfFire => 1.5;

	protected override DamageData DamageToApply => new ()
	{
		Damage = 30,
		Knockback = new Vector2((int)this.Bearing * 300, 0),
	};

		protected override double Health { get; set; } = 100;

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
	}
}

