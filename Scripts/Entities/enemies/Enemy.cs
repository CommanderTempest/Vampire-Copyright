using System.Runtime.CompilerServices;
using Godot;

public partial class Enemy : Entity
{
	[Signal]
	public delegate void EnemyDeathEventHandler(Enemy enemy);

	[Export] public int Damage = 1;

	private Node2D player;
	private bool canDamage = true;

	private Sprite2D sprite;

	public override void _Ready()
	{
		base._Ready();
		player = GetTree().CurrentScene.GetNodeOrNull<Node2D>("Player");
		sprite = GetNode<Sprite2D>("Sprite2D");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!IsInstanceValid(player))
			return;
		
		if (sprite != null)
		{
			sprite.Rotate(-0.05f);
		}

		Vector2 direction = (player.GlobalPosition - GlobalPosition).Normalized();
		Velocity = direction * Speed;
		MoveAndSlide();
	}

	// this is a signal placed on the Engine
	private async void OnHitboxBodyEntered(Node body)
	{
		if (body.HasMethod("TakeDamage") && canDamage)
		{
			canDamage = false;
			body.Call("TakeDamage", Damage);
			await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);
			canDamage = true;
		}
	}
	
	public void die()
	{
		EmitSignal(SignalName.EnemyDeath, this);
	}

	protected override void entityDeath()
	{
		EmitSignal(SignalName.EnemyDeath, this);
	}
}
