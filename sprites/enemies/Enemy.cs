using Godot;

public partial class Enemy : CharacterBody2D
{
	[Export] public float Speed = 120.0f;
	[Export] public int Damage = 1;

	private Node2D player;
	private bool canDamage = true;

	public override void _Ready()
	{
		player = GetTree().CurrentScene.GetNodeOrNull<Node2D>("Player");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!IsInstanceValid(player))
			return;

		Vector2 direction = (player.GlobalPosition - GlobalPosition).Normalized();
		Velocity = direction * Speed;
		MoveAndSlide();
	}

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
}
