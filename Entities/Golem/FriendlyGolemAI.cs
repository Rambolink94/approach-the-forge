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
		base._PhysicsProcess(delta);

		if (SearchForEnemies())
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

		if (SearchForEnemies())
		{
			this.AttackTargetInRange();
		}
		else
		{
			this.FaceObjective();
		}
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
