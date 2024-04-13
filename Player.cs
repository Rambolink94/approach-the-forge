using Godot;

namespace ApproachTheForge;

public partial class Player : CharacterBody2D
{
	[Export] private bool _overrideGravity;
	[Export] private float _gravityOverride = 10;
	[Export] private float _maxSpeed = 10;
	[Export] private float _maxSprintSpeed = 10;
	[Export] private float _acceleration = 5;
	[Export] private float _sprintAcceleration = 10;
	[Export] private float _deceleration = 50;
	[Export] private float _jumpVelocity = 100;
	
	private readonly float _defaultGravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	
	private Vector2 _input;
	private bool _isSprinting;
	private float _gravity;
	private Vector2 _velocity;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_gravity = _overrideGravity ? _gravityOverride : _defaultGravity;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		HandleInput();
	}

	public override void _PhysicsProcess(double delta)
	{
		_velocity.Y += _gravity * (float)delta;
		GD.Print(_velocity);
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

	private void HandleInput()
	{
		var input = new Vector2();
		if (Input.IsActionPressed("player_left"))
			input.X = -1;
		if (Input.IsActionPressed("player_right"))
			input.X = 1;

		_input = input.Normalized();
		if (Input.IsActionJustPressed("player_jump") && IsOnFloor())
		{
			GD.Print("JUMP");
			_velocity.Y = -_jumpVelocity;
		}

		if (Input.IsActionJustPressed("player_stealth_sprint"))
		{
			_isSprinting = !_isSprinting;
		}
	}
}