using System.Linq;

namespace ApproachTheForge.Entities.Golem {

	public partial class EnemyGolemAI : GolemAI
	{
		protected override Bearing ObjectiveBearing => Bearing.Left;

		public override void _PhysicsProcess(double delta)
		{
			this._Velocity = Velocity;

			// Add the gravity.
			if (!IsOnFloor())
			{
				this._Velocity.Y += Gravity * (float)delta;
			}

			if(SearchForPlayerEntity())
			{
				this.FaceTarget();
			}
			else
			{
				this.FaceObjective();
			}

			if(this.FindWalls())
			{
				this.Jump();
			}

			this._Velocity.X = (int)this.Bearing * Speed;

			this.Velocity = this._Velocity;

			MoveAndSlide();
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




