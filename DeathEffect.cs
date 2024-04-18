using Godot;

namespace ApproachTheForge;

public partial class DeathEffect : GpuParticles2D
{
	private AudioStreamPlayer2D _deathAudioPlayer;
	private bool _finished;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_deathAudioPlayer = GetNode<AudioStreamPlayer2D>("DeathAudioPlayer");
		_deathAudioPlayer.Play();
		Finished += () => _finished = true;
		Restart();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!_deathAudioPlayer.Playing && _finished)
		{
			QueueFree();
		}
	}
}