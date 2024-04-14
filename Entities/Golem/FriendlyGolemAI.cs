using ApproachTheForge;
using Godot;
using System;
using System.Linq;

public partial class FriendlyGolemAI : GolemAI
{
	protected override Bearing ObjectiveBearing => Bearing.Right;

	protected override double RateOfFire => 1.5;

	protected override DamageData DamageToApply => new DamageData()
	{
		Damage = 30,
		Knockback = new Vector2((int)this.Bearing * 300, 0),
	};

	protected override double Health { get; set; } = 100;

	public override void _PhysicsProcess(double delta)
	{
		this._Velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			this._Velocity.Y += gravity * (float)delta;
		}

		if (SearchForEnemies())
		{
			this.FaceTarget();

			this.AttackTargetInRange();
		}
		else
		{
			this.FaceObjective();
		}

		if (this.FindWalls())
		{
			this.Jump();
		}

		this._Velocity.X = this.CurrentState == GolemStates.Walking ? (int)this.Bearing * Speed : 0;

		this.Velocity = this._Velocity + this.TotalKnockback;

		this.TotalKnockback = new Vector2();


		MoveAndSlide();
		ManageAnimation();
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
