using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 500.0f;

	private bool attacking = false;

	private Tween tween;

	private Node2D attackPivot;
	private Area2D attackArea;
	private Sprite2D slashSprite;
	private Timer attackTimer;
	private ArrayList power_list;

	private HealthComponent healthComponent;
	private HealthUiComponent healthUIComponent;

	public override void _Ready()
	{
		this.initializeNodes();
		this.healthUIComponent.SetMaxHealth(this.healthComponent.MAX_HEALTH); // initialize to max health
		this.healthUIComponent.changeHealthUI(this.healthComponent.getHealth()); 
		
		attackArea.Monitoring = false;
		power_list = new ArrayList(); // TODO: figure out some other C# collection that allows dynamic sizing

		// 🔥 IMPORTANT: start hidden
		slashSprite.Visible = false;
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

	// a method to hold Node initializations
	public void initializeNodes()
	{
		attackPivot = GetNode<Node2D>("AttackPivot");
		attackArea = GetNode<Area2D>("AttackPivot/AttackArea");
		slashSprite = GetNode<Sprite2D>("AttackPivot/SlashSprite");
		attackTimer = GetNode<Timer>("Timer");
		healthComponent = GetNode<HealthComponent>("HealthComponent");
		healthUIComponent = GetNode<HealthUiComponent>("HealthUiComponent");
	}

	public void pickup_power(Power power)
	{
		power_list.Add(power);
		power.execute_on_pickup();
	}

	private void updateEveryPower()
	{
		// foreach (Power pow in power_list)
		// {
		// 	if (pow.execute_every_tick)
		// 	{
		// 		pow.update();
		// 	}
		// }
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
		GD.Print(this.healthComponent.getHealth());
		this.healthUIComponent.changeHealthUI(this.healthComponent.getHealth());

		if (this.healthComponent.getHealth() <= 0) {Die();}
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
