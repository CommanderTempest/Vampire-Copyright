using Godot;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 200.0f;
	[Export] public int MaxHp = 3;
	[Export] public float AttackDistance = 40.0f;

	private int hp;
	private bool attacking = false;

	private Node2D attackPivot;
	private Area2D attackArea;
	private Sprite2D slashSprite;
	private Timer attackTimer;

	public override void _Ready()
	{
		hp = MaxHp;

		attackPivot = GetNode<Node2D>("AttackPivot");
		attackArea = GetNode<Area2D>("AttackPivot/AttackArea");
		slashSprite = GetNode<Sprite2D>("AttackPivot/SlashSprite");
		attackTimer = GetNode<Timer>("Timer");

		attackArea.Monitoring = false;
		attackArea.Position = new Vector2(AttackDistance, 0);
		slashSprite.Position = new Vector2(AttackDistance, 0);
		slashSprite.Visible = false;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

		Velocity = attacking ? Vector2.Zero : direction * Speed;
		MoveAndSlide();
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_accept") && !attacking)
			Attack();
	}

	private async void Attack()
	{
		attacking = true;

		Vector2 mouseDirection = (GetGlobalMousePosition() - GlobalPosition).Normalized();
		if (mouseDirection == Vector2.Zero)
			mouseDirection = Vector2.Right;

		attackPivot.Rotation = mouseDirection.Angle();

		attackArea.Position = new Vector2(AttackDistance, 0);
		slashSprite.Position = new Vector2(AttackDistance, 0);

		attackArea.Monitoring = true;
		slashSprite.Visible = true;

		GD.Print("Attack shown");

		attackTimer.Start();
		await ToSignal(attackTimer, Timer.SignalName.Timeout);

		attackArea.Monitoring = false;
		slashSprite.Visible = false;
		attacking = false;
	}

	public void take_damage(int amount)
	{
		hp -= amount;
		GD.Print("Player HP: ", hp);

		if (hp <= 0)
			Die();
	}

	private void Die()
	{
		GetParent().Call("ShowGameOver");
		QueueFree();
	}

private void OnAttackAreaBodyEntered(Node body)
{
	if (body.Name == "Enemy")
	{
		GetParent().Call("AddScore", 1);
		body.QueueFree();
	}
}
}
