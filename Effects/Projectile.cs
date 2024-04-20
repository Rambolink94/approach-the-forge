using Godot;

namespace ApproachTheForge.Effects;

public partial class Projectile : Area2D
{
    [Export] private float _speed = 100f;
    [Export] private float _maxLifetime = 10f;
    
    public delegate void HitEventHandler(Projectile originator, IDamageable damageable, Vector2 hit);
    public event HitEventHandler Hit;

    private Node2D _target;
    private Vector2 _lastDirection;
    private bool _targetGenerated;
    private float _lifetime;

    public override void _Ready()
    {
        BodyEntered += OnHit;
    }

    public override void _Process(double delta)
    {
        _lifetime += (float)delta;
        if (_lifetime >= _maxLifetime)
        {
            OnHit(null);
        }

        if (!IsInstanceValid(_target) && !_targetGenerated)
        {
            // Generate new target to allow continued movement
            _target = new Node2D();
            _target.GlobalPosition = _lastDirection.Normalized() * 100f;
            _targetGenerated = true;
        }

        _lastDirection = _target.GlobalPosition - GlobalPosition;
            
        GlobalPosition = GlobalPosition.Lerp(_target.GlobalPosition, _speed * (float)delta);
    }

    public void SetTarget(Node2D target)
    {
        _target = target;
    }
    
    public virtual void OnHit(Node2D node)
    {
        if (node is null)
        {
            Hit?.Invoke(this, null, default);   // Is this the best way to do this?
            QueueFree();
            return;
        }
        
        if (node is IDamageable entity)
        {
            Vector2 direction = (node.GlobalPosition - GlobalPosition).Normalized();
            Hit?.Invoke(this, entity, direction);
        }
    }
}