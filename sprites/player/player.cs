using Godot;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 500.0f;
	[Export] public int MaxHp = 3;

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

		// 🔥 IMPORTANT: start hidden
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
		if (Input.IsActionJustPressed("attack") && !attacking)
			Attack();
	}

	private async void Attack()
	{
		attacking = true;

		Vector2 mouseDirection = (GetGlobalMousePosition() - GlobalPosition).Normalized();
		if (mouseDirection == Vector2.Zero)
			mouseDirection = Vector2.Right;

		// ONLY rotate pivot → keeps your exact Inspector transformation
		attackPivot.Rotation = mouseDirection.Angle();

		attackArea.Monitoring = true;
		slashSprite.Visible = true;

		GD.Print("Attack shown");

		attackTimer.Start();
		await ToSignal(attackTimer, Timer.SignalName.Timeout);

		attackArea.Monitoring = false;
		slashSprite.Visible = false;
		attacking = false;
	}

	public void TakeDamage(int amount)
	{
		hp -= amount;
		GD.Print("Player HP: ", hp);

		if (hp <= 0)
			Die();
	}

	private void Die()
	{
		GD.Print("PLAYER DIED");
		GetTree().CurrentScene.Call("ShowGameOver");
		SetPhysicsProcess(false);
		Visible = false;
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
