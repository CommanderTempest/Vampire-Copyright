using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

public partial class player : CharacterBody2D
{
	[Export] public float Speed = 500.0f;

	private int hp;
	private bool attacking = false;

	private Tween tween;

	private Node2D attackPivot;
	private Area2D attackArea;
	private Sprite2D slashSprite;
	private Timer attackTimer;
	private TextureProgressBar healthBar;
	private ArrayList power_list;

	private HealthComponent healthComponent;

	public override void _Ready()
	{
		attackPivot = GetNode<Node2D>("AttackPivot");
		attackArea = GetNode<Area2D>("AttackPivot/AttackArea");
		slashSprite = GetNode<Sprite2D>("AttackPivot/SlashSprite");
		attackTimer = GetNode<Timer>("Timer");
		healthBar = GetNode<TextureProgressBar>("../UI/HealthBar");
		healthBar.Value = hp;
		healthComponent = new HealthComponent(); // TODO: change to grab the node

		attackArea.Monitoring = false;
		power_list = new ArrayList(); // TODO: figure out some other C# collection that allows dynamic sizing

		// 🔥 IMPORTANT: start hidden
		slashSprite.Visible = false;
		healthBar.Visible = true;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

		// tentatively deleted this, why should the player stop moving for attacks
		//Velocity = attacking ? Vector2.Zero : direction * Speed;
		Velocity = direction * Speed;
		MoveAndSlide();
		updateEveryPower();
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("attack") && !attacking)
			Attack();
	}

	public void pickup_power(Power power)
	{
		power_list.Add(power);
		power.execute_on_pickup();
	}

	private void updateEveryPower()
    {
        foreach (Power pow in power_list)
		{
			if (pow.execute_every_tick)
			{
				pow.update();
			}
		}
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
		this.healthComponent.reduceHealth(amount);
		GD.Print("Player HP: ", hp);
		//updateHealthUI();

		if (this.healthComponent.getHealth() <= 0) {Die();}
	}
	
	// private void updateHealthUI()
	// {
	// 	Tween tween = GetTree().CreateTween().SetTrans(Tween.TransitionType.Sine);
	// 	if (tween != null)
	// 	{
	// 		tween.Kill();
	// 	}
	// 	tween = GetTree().CreateTween().SetTrans(Tween.TransitionType.Sine);
	// 	tween.TweenProperty(healthBar, "value", this.hp, 1);
	// }

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
