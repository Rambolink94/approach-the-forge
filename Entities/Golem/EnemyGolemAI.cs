using Godot;
using Godot.Collections;
using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace ApproachTheForge {

	public partial class EnemyGolemAI : GolemAI
	{
		protected override Bearing ObjectiveBearing => Bearing.Left;

		public override void _PhysicsProcess(double delta)
		{
			this._Velocity = Velocity;

			// Add the gravity.
			if (!IsOnFloor())
			{
				this._Velocity.Y += gravity * (float)delta;
			}

			if(SearchForPlayerEntity())
			{
				this.FaceTarget();

				this.AttackTargetInRange();
			}
			else
			{
				this.FaceObjective();
			}

			if(this.FindWalls())
			{
				this.Jump();
			}

			this._Velocity.X = this.CurrentState == GolemStates.Walking ? (int)this.Bearing * Speed : 0;

			this.Velocity = this._Velocity;

			MoveAndSlide();
			ManageAnimation();
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




