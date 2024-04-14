using Godot;
using System;

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

		protected Node2D Target;

		protected abstract Bearing ObjectiveBearing { get; }

		/// <summary>
		///  Components fields
		/// </summary>
		protected Area2D WallDetector;

		protected Area2D DetectionArea;

		protected Area2D AttackRange;

		protected AnimatedSprite2D AnimatedSprite;

		protected GolemStates CurrentState;

		// Get the gravity from the project settings to be synced with RigidBody nodes.
		public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

		public override void _Ready()
		{
			base._Ready();

			this.DetectionArea = GetNode<Area2D>("Detection Area");

			this.AttackRange = this.DetectionArea.GetNode<Area2D>("Attack Range");

			this.WallDetector = this.GetNode<Area2D>("Wall Detector");

			this.AnimatedSprite = this.GetNode<AnimatedSprite2D>("Animated Golem");
		}

		protected void AttackTargetInRange()
		{
			if (this.AttackRange.GetOverlappingBodies().Contains(this.Target))
			{
				this.CurrentState = GolemStates.Attacking;
			}
			else
			{
				this.CurrentState = GolemStates.Walking;
			}
		}

		protected bool FindWalls()
		{
			if(this.WallDetector.HasOverlappingBodies())
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
			float direction = this.Target.Position.X - this.Position.X;

			this.Bearing = direction > 0 ? Bearing.Right : Bearing.Left;

			this.WallDetector.Scale = new Vector2((int)this.Bearing, 1);		
		}

		protected void FaceObjective()
		{
			this.Bearing = this.ObjectiveBearing;

			this.WallDetector.Scale = new Vector2((int)this.Bearing, 1);
		}

		protected void ManageAnimation()
		{
			// The negative 1 is because the drawn animation is facing to the left to start.
			this.AnimatedSprite.Scale = new Vector2((int)this.Bearing * -1, 1);
			switch (this.CurrentState)
			{
				case GolemStates.Walking:
					this.AnimatedSprite.Play("Golem Walk");
					break;
				case GolemStates.Attacking:
					this.AnimatedSprite.Play("Golem Attack");
					break;
				case GolemStates.Jumping:
					break;
			}
		}

	}
}

