using Godot;

public partial class Enemy : CharacterBody2D
{
	[Export] public float Speed = 120.0f;
	[Export] public int Damage = 1;

	private Node2D player;
	private bool canDamage = true;

	public override void _Ready()
	{
		player = GetTree().CurrentScene.GetNode<Node2D>("Player");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (player == null)
			return;

		Vector2 direction = (player.GlobalPosition - GlobalPosition).Normalized();
		Velocity = direction * Speed;
		MoveAndSlide();
	}

	private async void OnHitboxBodyEntered(Node body)
	{
		if (body.HasMethod("take_damage") && canDamage)
		{
			canDamage = false;
			body.Call("take_damage", Damage);
			await ToSignal(GetTree().CreateTimer(1.0f), SceneTreeTimer.SignalName.Timeout);
			canDamage = true;
		}
	}
}
