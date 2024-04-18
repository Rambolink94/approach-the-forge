using ApproachTheForge.Pickups;
using ApproachTheForge.Utility;
using Godot;

namespace ApproachTheForge.Entities.Player;

public partial class Player : Entity, IDamageable
{
	[Export] private bool _overrideGravity;
	[Export] private float _gravityOverride = 10;
	[Export] private float _maxSpeed = 10;
	[Export] private float _maxSprintSpeed = 10;
	[Export] private float _acceleration = 5;
	[Export] private float _sprintAcceleration = 10;
	[Export] private float _deceleration = 50;
	[Export] private float _jumpVelocity = 100;
	
	[ExportCategory("Damage Data")]
	[Export] private float _damage = 10f;
	[Export] private float _knockback = 100f;
	[Export] private float _attackSpeed = 0.1f;
	
	public float Health { get; private set; }
	
	private readonly float _defaultGravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	
	private Vector2 _input;
	private bool _isSprinting;
	private float _gravity;
	private Vector2 _velocity;
	private Sprite2D _sprite;
	private GpuParticles2D _jumpPuff;
	private Area2D _collectionArea;
	private Area2D _damageArea;
	private bool _flipState;
	private float _timeSinceLastAttack;
	private bool _attackReady;
	private AudioStreamPlayer2D _attackAudioPlayer;
	private GpuParticles2D _attackParticle;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_gravity = _overrideGravity ? _gravityOverride : _defaultGravity;
		_sprite = GetNode<Sprite2D>("PlayerArt");
		_jumpPuff = GetNode<GpuParticles2D>("JumpPuff");
		_collectionArea = GetNode<Area2D>("CollectionArea");
		_damageArea = GetNode<Area2D>("DamageArea");
		_attackAudioPlayer = GetNode<AudioStreamPlayer2D>("AttackPlayer");
		_attackParticle = GetNode<GpuParticles2D>("DamageArea/AttackParticle");

		_collectionArea.BodyEntered += OnAreaEntered;
		AbilityController.AbilityChanged += (action, _) =>
		{
			if (action == "player_stealth_sprint")
			{
				_isSprinting = !_isSprinting;
			}
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_timeSinceLastAttack >= _attackSpeed)
		{
			_attackReady = true;
			_timeSinceLastAttack = 0;
		}

		_timeSinceLastAttack += (float)delta;
		
		HandleInput();
	}

	public override void _PhysicsProcess(double delta)
	{
		_velocity.Y += _gravity * (float)delta;
		var horizontalVelocity = _velocity;
		horizontalVelocity.Y = 0;

		var speed = _isSprinting ? _maxSprintSpeed : _maxSpeed;
		var acceleration = _isSprinting ? _sprintAcceleration : _acceleration;
		
		Vector2 target = _input * speed;
		if (_input.Dot(horizontalVelocity) <= 0)
		{
			acceleration = _deceleration;
		}

		_velocity.X = Mathf.Lerp(_velocity.X, target.X, acceleration * (float)delta);

		Velocity = _velocity;
		bool collisionOccured = MoveAndSlide();
		if (collisionOccured)
		{
		}
		
		_velocity = Velocity;
	}

	protected override void Die(bool removeOnDeath = true)
	{
		base.Die(removeOnDeath);
		
		GD.Print("You Died!");
	}

	private void OnAreaEntered(Node2D resource)
	{
		if (resource is not ResourcePickup pickup) return;
		
		pickup.Collect();
	}

	private void HandleInput()
	{
		var input = new Vector2();
		if (Input.IsActionPressed("player_left"))
		{
			input.X = -1;
			Flip(input);
		}

		if (Input.IsActionPressed("player_right"))
		{
			input.X = 1;
			Flip(input);
		}

		_input = input.Normalized();
		if (Input.IsActionJustPressed("player_jump") && IsOnFloor())
		{
			_jumpPuff.Restart();
			_velocity.Y = -_jumpVelocity;
		}

		if (Input.IsActionPressed("player_action")
		    && !GameManager.PlacementController.IsActive
		    && _attackReady)
		{
			_attackReady = false;
			Attack();
		}
	}

	public bool ApplyDamage(DamageData damageInstance)
	{
		Health -= damageInstance.Damage;
		if (Health <= 0)
		{
			Die(false);

			return true;
		}
		
		return false;
	}

	private void Attack()
	{
		foreach (Node2D body in _damageArea.GetOverlappingBodies())
		{
			if (body is IDamageable damageable)
			{
				Vector2 direction = (body.GlobalPosition - GlobalPosition).Normalized();
				var data = new DamageData
				{
					Damage = _damage,
					Knockback = direction * _knockback,
				};
					
				damageable.ApplyDamage(data);
			}
		}
		
		_attackParticle.Restart();
		_attackAudioPlayer.Play();
	}
    
	private void Flip(Vector2 direction)
	{
		var currentDirection = _damageArea.Position.Normalized();
		var dot = currentDirection.Dot(direction);
		if (currentDirection.Dot(direction) < 1)
		{
			_sprite.FlipH = !_sprite.FlipH;
			Vector2 pos = _damageArea.Position;
			pos.X *= -1;
			_damageArea.Position = pos;
		}
	}
}


