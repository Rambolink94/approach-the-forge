using Godot;

namespace ApproachTheForge.Effects;

public partial class EnergyProjectile : Projectile
{
	private AudioStreamPlayer2D _hitSound;
	private bool _beenHit;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_hitSound = GetNode<AudioStreamPlayer2D>("ExplosionSound");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		
		if (_beenHit && !_hitSound.Playing)
		{
			QueueFree();
		}
	}
	
	public override void OnHit(Node2D node)
	{
		_beenHit = true;
		base.OnHit(node);
		_hitSound.Play();
	}
}