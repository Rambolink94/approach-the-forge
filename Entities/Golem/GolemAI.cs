using Godot;
using System;
using System.Runtime.InteropServices.ObjectiveC;

namespace ApproachTheForge
{
	public enum Bearing
	{
		Left = -1,
		Right = 1,
	}
	public abstract partial class GolemAI : CharacterBody2D
	{
		public const float Speed = 100.0f;
		public const float JumpVelocity = -100.0f;

		protected Bearing Bearing = Bearing.Left;

		protected Vector2 _Velocity = new Vector2();

		protected Node2D Target = new Node2D();

		protected abstract Bearing ObjectiveBearing { get; }

		// Get the gravity from the project settings to be synced with RigidBody nodes.
		public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

		protected bool FindWalls()
		{
			Area2D wallDetector = GetNode<Area2D>("Wall Detector");

			if(wallDetector.HasOverlappingBodies())
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		protected void Jump()
		{
			this._Velocity.Y += JumpVelocity;
		}

		protected void FaceTarget()
		{
			Area2D wallDetector = GetNode<Area2D>("Wall Detector");

			
			float direction = this.Target.Position.X - this.Position.X;

			this.Bearing = direction > 0 ? Bearing.Right : Bearing.Left;

			wallDetector.Scale = new Vector2((int)this.Bearing, 1);		
		}

		protected void FaceObjective()
		{
			this.Bearing = this.ObjectiveBearing;

			Area2D wallDetector = GetNode<Area2D>("Wall Detector");

			wallDetector.Scale = new Vector2((int)this.Bearing, 1);
		}
	}
}

