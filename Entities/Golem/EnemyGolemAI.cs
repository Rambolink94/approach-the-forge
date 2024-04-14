using Godot;
using Godot.Collections;
using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace ApproachTheForge {
	public enum Bearing
	{
		Left = -1,
		Right = 1,
	}

	public partial class EnemyGolemAI : CharacterBody2D
	{
		public const float Speed = 100.0f;

		public Bearing Bearing = Bearing.Left;

		public int DetectionRange = 200;

		// Get the gravity from the project settings to be synced with RigidBody nodes.
		public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

		public override void _PhysicsProcess(double delta)
		{
			Vector2 velocity = Velocity;

			// Add the gravity.
			if (!IsOnFloor())
			{
				velocity.Y += gravity * (float)delta;
			}

			SearchForPlayerEntity();

			velocity.X = (int)this.Bearing * Speed;

			this.Velocity = velocity;

			MoveAndSlide();
		}

		private void SearchForPlayerEntity()
		{
			Area2D detectionArea = GetNode<Area2D>("DetectionArea");

			if (detectionArea.HasOverlappingBodies())
			{
				Node2D firstTarget = detectionArea.GetOverlappingBodies().FirstOrDefault();

				float direction = firstTarget.Position.X - this.Position.X;

				this.Bearing = direction > 0 ? Bearing.Right : Bearing.Left;
			}
		}
	}
}




