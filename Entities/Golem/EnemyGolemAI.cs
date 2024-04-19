using Godot;
using System.Linq;

namespace ApproachTheForge.Entities.Golem
{
	public partial class EnemyGolemAI : GolemAI
	{
		protected override Bearing ObjectiveBearing => Bearing.Left;

		protected override double RateOfFire => 1.5;

		protected override DamageData DamageToApply => new DamageData() 
		{
			Damage = 30,
			Knockback = new Vector2((int)this.Bearing * 300, 0),
		};

		public override float Health { get; protected set; } = 100;

		public override void _PhysicsProcess(double delta)
		{
			base._PhysicsProcess(delta);

			if (SearchForPlayerEntity())
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

			if (SearchForPlayerEntity())
			{
				this.AttackTargetInRange();
			}
			else
			{
				this.FaceObjective();
			}
		}

		private bool SearchForPlayerEntity()
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




