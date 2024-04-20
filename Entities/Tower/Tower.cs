using System.Collections.Generic;
using System.Linq;
using ApproachTheForge.Effects;
using ApproachTheForge.Utility;
using Godot;

namespace ApproachTheForge.Entities.Tower;

public partial class Tower : Node2D, IPlaceable, IDamageable
{
	[Export] public float Health { get; private set; } = 300f;

	[ExportCategory("Damage Data")]
	[Export] private float _damage = 40;
	[Export] private float _knockback = 300;
	[Export] private float _attackSpeed = 1f;
	[Export] private float _attackDelay = 0.2f;

	private PackedScene _projectileEffect = GD.Load<PackedScene>("res://Effects/projectile_effect.tscn");

	private Area2D _detectionArea;
	private GpuParticles2D _preShotParticle;
	private AudioStreamPlayer2D _preShotAudio;
	private AudioStreamPlayer2D _shotAudio;
	private ProgressBar _healthBar;
	private float _timeSinceLastAttack;
	private float _currentAttackDelay;
	private Node2D _parent;
	private bool _isDead;

	private readonly List<Projectile> _activeProjectiles = new();
	private readonly List<Node2D> _trackedEnemies = new();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_detectionArea = GetNode<Area2D>("DetectionArea");
		_preShotParticle = GetNode<GpuParticles2D>("PreShotParticle");
		_preShotAudio = GetNode<AudioStreamPlayer2D>("PreShotParticle/PreShotAudio");
		_shotAudio = GetNode<AudioStreamPlayer2D>("PreShotParticle/ShotAudio");

		_preShotParticle.Emitting = false;

		_detectionArea.BodyEntered += OnEnemyDetected;
		_detectionArea.BodyExited += OnEnemyLost;
		
		_healthBar = GetNode<ProgressBar>("HealthBar");
		_healthBar.MaxValue = Health;
		_healthBar.Value = Health;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_timeSinceLastAttack += (float)delta;
		if (_timeSinceLastAttack >= _attackSpeed && _trackedEnemies.Count > 0)
		{
			if (!_preShotParticle.Emitting)
			{
				_preShotAudio.Play();
				_preShotParticle.Restart();
			}
			
			_currentAttackDelay += (float)delta;
			if (_currentAttackDelay >= _attackDelay && _trackedEnemies.Count > 0)
			{
				// TODO: Write enemy selection algorithm
				var target = _trackedEnemies[0];
				
				var projectile = _projectileEffect.Instantiate<EnergyProjectile>();
				projectile.Position = _preShotParticle.Position;
				projectile.SetTarget(target);
				projectile.Hit += OnProjectileHit;
				_activeProjectiles.Add(projectile);
				AddChild(projectile);
				
				_shotAudio.Play();

				_timeSinceLastAttack = 0;
				_currentAttackDelay = 0;
			}
		}
		

		if (_isDead && _activeProjectiles.Count == 0)
		{
			QueueFree();
		}
	}
	
	public bool ApplyDamage(DamageData damageData)
	{
		Health -= damageData.Damage;
		_healthBar.Value = Health;
		if (Health <= 0)
		{
			Die();
			return true;
		}

		return false;
	}

	private void Die()
	{
		_isDead = true;
		GD.Print("Tower died!");
	}

	private void OnEnemyDetected(Node2D enemy)
	{
		_trackedEnemies.Add(enemy);
	}

	private void OnEnemyLost(Node2D enemy)
	{
		_trackedEnemies.Remove(enemy);
	}
	
	private void OnProjectileHit(Projectile originator, IDamageable hitEntity, Vector2 hitNormal)
	{
		_activeProjectiles.Remove(originator);
		hitEntity?.ApplyDamage(new DamageData
		{
			Damage = _damage,
			Knockback = hitNormal * _knockback,
		});
	}
}
