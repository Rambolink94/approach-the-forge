using Godot;
using System.Linq;

namespace ApproachTheForge.Entities.Golem
{
	public partial class EnemyGolemAI : GolemAI
	{
		protected override Bearing ObjectiveBearing => Bearing.Left;

		protected override DamageData DamageToApply => new DamageData()
		{
			Damage = (float)this.Damage,
			Knockback = new Vector2((int)this.Bearing * 300, 0),
		};

		protected override double MaxHealth { get; } = 100;

		protected override double Health { get; set; }

		protected override double Damage { get; } = 30;

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




