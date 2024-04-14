using ApproachTheForge.Utility;
using Godot;

namespace ApproachTheForge.Entities.Golem
{
	public enum Bearing
	{
		Left = -1,
		Right = 1,
	}
	public abstract partial class GolemAI : Entity
	{
		public const float Speed = 100.0f;
		public const float JumpVelocity = -100.0f;

		protected Bearing Bearing = Bearing.Left;

		protected Vector2 _Velocity = new Vector2();

		protected Node2D Target = new Node2D();
		
		protected abstract Bearing ObjectiveBearing { get; }

		protected Area2D WallDetector;

		protected Area2D DetectionArea;

		// Get the gravity from the project settings to be synced with RigidBody nodes.
		protected float Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
		
		public override void _Ready()
		{
			base._Ready();

			this.WallDetector = GetNode<Area2D>("Wall Detector");

			this.DetectionArea = GetNode<Area2D>("Detection Area");
		}

		protected bool FindWalls()
		{
			return this.WallDetector.HasOverlappingBodies();
		}

		protected void Jump()
		{
			this._Velocity.Y += JumpVelocity;
		}

		protected void FaceTarget()
		{
			float direction = this.Target.Position.X - this.Position.X;

			this.Bearing = direction > 0 ? Bearing.Right : Bearing.Left;

			this.WallDetector.Scale = new Vector2((int)this.Bearing, 1);		
		}

		protected void FaceObjective()
		{
			this.Bearing = this.ObjectiveBearing;

			this.WallDetector.Scale = new Vector2((int)this.Bearing, 1);
		}
	}
}

