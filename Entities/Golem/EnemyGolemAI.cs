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
			Area2D detectionArea = GetNode<Area2D>("Detection Area");

			if (detectionArea.HasOverlappingBodies())
			{
				this.Target = detectionArea.GetOverlappingBodies().FirstOrDefault();

				return true;
			}
			else
			{
				return false;
			}
		}
	}
}




