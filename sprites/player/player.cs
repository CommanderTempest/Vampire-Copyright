using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

public partial class player : CharacterBody2D
{
	[Export] public float Speed = 500.0f;
	private AnimationTree _animationTree;
	
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
	private AnimationNodeStateMachinePlayback _playback;

	private bool shieldPowerActive = false;


	public override void _Ready()
	{
		this.initializeNodes();
		this.healthUIComponent.SetMaxHealth(this.healthComponent.MAX_HEALTH); // initialize to max health
		this.healthUIComponent.changeHealthUI(this.healthComponent.getHealth()); 
		
		attackArea.Monitoring = false;
		power_list = new ArrayList(); // TODO: figure out some other C# collection that allows dynamic sizing

		// 🔥 IMPORTANT: start hidden
		slashSprite.Visible = false;

		//this.pickup_power(new ShieldPower());
		_animationTree = GetNode<AnimationTree>("Player/AnimationTree");
		_animationTree.Active = true;
		_playback = (AnimationNodeStateMachinePlayback)_animationTree.Get("parameters/playback");
		// this will set 
	}
	private void UpdateAnimationParameters()
{
	Vector2 direction = Velocity;

	// Prevent jitter when standing still
	if (direction.Length() > 0)
		direction = direction.Normalized();

	_animationTree.Set("parameters/Idle/blend_position", direction);
	_animationTree.Set("parameters/Run/blend_position", direction);

   /* if (attacking)
	{
		_playback.Travel("Attack");
	}
	else if 
	TODO will add for attacks in the future */ 
	if (direction.Length() > 0)
	{
		_playback.Travel("Run");
	}
	else
	{
		_playback.Travel("Idle");
	}
}

	public override void _PhysicsProcess(double delta)
	{
		
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		
		// tentatively deleted this, why should the player stop moving for attacks
		//Velocity = attacking ? Vector2.Zero : direction * Speed;
		Velocity = direction * Speed;
		MoveAndSlide();
		UpdateAnimationParameters();
		// this will update the charaters animation
		
	}
	
	
	
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("attack") && !attacking)
			Attack();
	}

	private void setShieldPowerActive(bool value)
	{
		if (!value) {this.shieldPower.shieldDeactivate();}
		this.shieldPowerActive = value;
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
			this.shieldPower.activate();
		}
		this.AddChild(this.shieldPower);
		power_list.Add(power);
	}

	private void activateShield()
	{
		setShieldPowerActive(true);
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
			setShieldPowerActive(false);
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
