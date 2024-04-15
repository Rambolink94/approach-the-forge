using ApproachTheForge.Utility;
using Godot;

namespace ApproachTheForge.Entities.Golem
{
	public enum Bearing
	{
		Left = -1,
		Right = 1,
	}
	public abstract partial class GolemAI : Entity, Damageable
	{
		public const float Speed = 100.0f;
		public const float JumpVelocity = -100.0f;

		// The direction the golem is facing
		protected Bearing Bearing = Bearing.Left;

		// An internal velocity for tracking updating the the velocity of the golem within a game tick
		protected Vector2 _Velocity = new Vector2();

		// If a target is found within this.Detection area, the reference to the closest target is saved here.
		protected Node2D Target;

		// The direction towards the game relative game objective. Left for hostiles and right for player entities.
		protected abstract Bearing ObjectiveBearing { get; }

		// The current state of the golem for managing animations
		protected GolemStates CurrentState;

		/// <summary>
		///		Components fields
		/// </summary>
		 
		// A collision box for detecting walls
		protected Area2D WallDetector;

		// A collision circle for detecting members of the opposing team
		protected Area2D DetectionArea;

		// A collision box representing the range at which the golem can attack
		protected Area2D AttackRange;

		// The animated sprite of the golem
		protected AnimatedSprite2D AnimatedSprite;

		// The timer used to limit the rate of fire
		protected Timer AttackTimer;

		// The timer used for delaying applying damage
		protected Timer AttackDelayTimer;

		/// <summary>
		///		Combat fields and properties
		/// </summary>
		
		// The rate of fire of the golem
		protected abstract double RateOfFire { get; }

		// A flag for whether the attack cooldown as passed
		protected bool CanAttack => this.AttackTimer.TimeLeft == 0;

		// A struct containing the damaging properties of the golem
		protected abstract DamageData DamageToApply { get; }

		// The health of the golem
		protected abstract double Health { get; set; }

		// The total knockback the golem as recieved in a game tick
		protected Vector2 TotalKnockback = new Vector2();

		protected bool IsAttacking = false;


		// Get the gravity from the project settings to be synced with RigidBody nodes.
		public float Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

		/// <summary>
		///		Called when the object first enters the scene.
		/// </summary>
		public override void _Ready()
		{
			base._Ready();

			this.DetectionArea = GetNode<Area2D>("Detection Area");

			this.AttackRange = this.DetectionArea.GetNode<Area2D>("Attack Range");

			this.WallDetector = this.GetNode<Area2D>("Wall Detector");

			this.AnimatedSprite = this.GetNode<AnimatedSprite2D>("Animated Golem");

			this.AttackTimer = this.GetNode<Timer>("Rate Of Fire Timer");

			this.AttackTimer.Start(this.RateOfFire);

			this.AttackDelayTimer = this.GetNode<Timer>("Attack Delay Timer");
		}

		/// <summary>
		///		Called every game tick for processing physics calculations
		/// </summary>
		/// <param name="delta"> The number of miliseconds passed since the last game cycle. </param>
		public override void _PhysicsProcess(double delta)
		{
			this._Velocity = Velocity;

			// Add the gravity.
			if (!IsOnFloor())
			{
				this._Velocity.Y += gravity * (float)delta;
			}

			if(this.FindWalls())
			{
				this.Jump();
			}

			this._Velocity.X = this.CurrentState == GolemStates.Walking ? (int)this.Bearing * Speed : 0;

			this.Velocity = this._Velocity + this.TotalKnockback;

			this.TotalKnockback = new Vector2();

			MoveAndSlide();
			ManageAnimation();
		}

		/// <summary>
		///		Called every game tick for processing
		/// </summary>
		/// <param name="delta"> The number of miliseconds passed since the last game cycle. </param>
		public override void _Process(double delta)
		{
			base._Process(delta);

			if(IsAttacking)
			{
				this.ApplyDamageOnDelay();
			}
		}

		/// <summary>
		///		If there is a target in range, attack it according to the rate of fire.
		/// </summary>
		protected void AttackTargetInRange()
		{
			if (this.AttackRange.GetOverlappingBodies().Contains(this.Target))
			{
				this.CurrentState = GolemStates.Attacking;

				if (CanAttack)
				{
					this.IsAttacking = true;

					// The strike is on the 7th frame out of the 10 frame animation and the rate of fire is 1:1 with the attack animation
					this.AttackDelayTimer.Start( 7 * this.RateOfFire / 10);
					this.AttackDelayTimer.Paused = false;

					this.AttackTimer.Start(this.RateOfFire);
					this.AttackTimer.Paused = false;
				}
			}
			else
			{
				this.CurrentState = GolemStates.Walking;
			}
		}

		protected void ApplyDamageOnDelay()
		{
			if (this.AttackDelayTimer.TimeLeft == 0)
			{
				if (this.Target is Damageable)
				{
					if ((this.Target as Damageable).ApplyDamage(this.DamageToApply))
					{
						// The target will remove itself from the game. Update the target reference
						this.Target = new Node2D();
					}
				}

				this.IsAttacking = false;
			}
		}

		/// <summary>
		///		Use this.WallDetector to search for path blocking terrain.
		/// </summary>
		/// <returns> True if a wall was found, false otherwise. </returns>
		protected bool FindWalls()
		{
			return this.WallDetector.HasOverlappingBodies();
		}

		/// <summary>
		///		Update this.Velocity to clear terrain obstacles.
		/// </summary>
		protected void Jump()
		{
			this._Velocity.Y += JumpVelocity;
		}

		/// <summary>
		///		Change bearing and wall detector to face the target
		/// </summary>
		protected void FaceTarget()
		{
			float direction = this.Target.Position.X - this.Position.X;

			this.Bearing = direction > 0 ? Bearing.Right : Bearing.Left;

			this.WallDetector.Scale = new Vector2((int)this.Bearing, 1);		
		}

		/// <summary>
		///		Change bearing and wall detector to face the objective
		/// </summary>
		protected void FaceObjective()
		{
			this.Bearing = this.ObjectiveBearing;

			this.WallDetector.Scale = new Vector2((int)this.Bearing, 1);
		}

		/// <summary>
		///		Play the relevant animation based on the current action of the golem.
		/// </summary>
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

		/// <summary>
		///		Recieve a Damage Data struct from an attacker and applying the changes due to damage.
		/// </summary>
		/// <param name="damageData"> A struct containing the necessary data for taking damage properly. </param>
		/// <returns> True if this object dies, false otherwise. </returns>
		public bool ApplyDamage(DamageData damageData)
		{
			this.TotalKnockback += damageData.Knockback;

			GD.Print("Damage Taken: True");
			GD.Print("Current HP: " + this.Health);

			if (this.Health - damageData.Damage <= 0)
			{
				this.Health = 0;

				this.QueueFree();

				return true;
			}
			else
			{
				this.Health -= damageData.Damage;
				return false;
			}
		}
	}
}

