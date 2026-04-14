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
	private ShieldPower shieldPower;

	private bool shieldPowerActive {
		get => shieldPowerActive;
		set
		{
			if (value == false)
			{
				this.shieldPower.shieldDeactivate();
			}
		} 
	}

	public override void _Ready()
	{
		this.initializeNodes();
		this.healthUIComponent.SetMaxHealth(this.healthComponent.MAX_HEALTH); // initialize to max health
		this.healthUIComponent.changeHealthUI(this.healthComponent.getHealth()); 
		
		attackArea.Monitoring = false;
		power_list = new ArrayList(); // TODO: figure out some other C# collection that allows dynamic sizing

		// 🔥 IMPORTANT: start hidden
		slashSprite.Visible = false;

		this.pickup_power(new ShieldPower());
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

		// tentatively deleted this, why should the player stop moving for attacks
		//Velocity = attacking ? Vector2.Zero : direction * Speed;
		Velocity = direction * Speed;
		MoveAndSlide();
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
		if (power is ShieldPower)
		{
			this.shieldPower = (ShieldPower) power;
			this.shieldPower.ShieldActivate += activateShield;
		}
		power_list.Add(power);
	}

	private void activateShield()
	{
		GD.Print("attempting to turn on at least");
		this.shieldPowerActive = true;
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
		if (!this.shieldPowerActive)
		{
			this.healthComponent.reduceHealth(amount);
			GD.Print(this.healthComponent.getHealth());
			this.healthUIComponent.changeHealthUI(this.healthComponent.getHealth());

			if (this.healthComponent.getHealth() <= 0) {Die();}
		}
		else
		{
			GD.Print("Shield Power blocked an attack");
			this.shieldPowerActive = false;
		}
		
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
