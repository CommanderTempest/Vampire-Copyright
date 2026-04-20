using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

public partial class player : CharacterBody2D
{
	[Export] public float Speed = 500.0f;

	private ArrayList power_list;
	private AnimatedSprite2D animatedSprite;
	private Vector2 lastDirection = Vector2.Down;
	private HealthComponent healthComponent;
	private HealthUiComponent healthUIComponent;
	private ShieldPower shieldPower;
	private bool shieldPowerActive = false;
	private AttackComponent attackComponent;

	public override void _Ready()
	{
		initializeNodes();

		healthUIComponent.SetMaxHealth(healthComponent.MAX_HEALTH);
		healthUIComponent.changeHealthUI(healthComponent.getHealth());

		power_list = new ArrayList();
		UpdateAnimation(Vector2.Zero);
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Velocity = direction * Speed;
		MoveAndSlide();
		UpdateAnimation(direction);
	}
	private void UpdateAnimation(Vector2 direction)
{
	if (animatedSprite == null)
		return;

	if (direction != Vector2.Zero)
		lastDirection = direction;

	string animationName;

	if (direction == Vector2.Zero)
	{
		if (Mathf.Abs(lastDirection.X) > Mathf.Abs(lastDirection.Y))
		{
			animationName = "Idle_side";
			animatedSprite.FlipH = lastDirection.X < 0;
		}
		else if (lastDirection.Y < 0)
		{
			animationName = "Idle_up";
			animatedSprite.FlipH = false;
		}
		else
		{
			animationName = "Idle_down";
			animatedSprite.FlipH = false;
		}
	}
	else
	{
		if (Mathf.Abs(direction.X) > Mathf.Abs(direction.Y))
		{
			animationName = "Run_side";
			animatedSprite.FlipH = direction.X < 0;
		}
		else if (direction.Y < 0)
		{
			animationName = "Run_up";
			animatedSprite.FlipH = false;
		}
		else
		{
			animationName = "Run_down";
			animatedSprite.FlipH = false;
		}
	}

	if (animatedSprite.Animation != animationName || !animatedSprite.IsPlaying())
		animatedSprite.Play(animationName);
}

	
	public override void _Process(double delta)
	{
		if (Input.IsMouseButtonPressed(MouseButton.Left))
			GD.Print("LEFT CLICK HELD");

		if (Input.IsActionJustPressed("attack"))
		{
			GD.Print("ATTACK ACTION FIRED");
			attackComponent.TryAttack();
		}
	}

	private void setShieldPowerActive(bool value)
	{
		if (!value)
			shieldPower.shieldDeactivate();

		shieldPowerActive = value;
	}

	public void initializeNodes()
	{
		attackComponent = GetNode<AttackComponent>("AttackComponent");
		healthComponent = GetNode<HealthComponent>("HealthComponent");
		healthUIComponent = GetNode<HealthUiComponent>("HealthUiComponent");
		animatedSprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
		
		Node spriteNode = GetNode("AnimatedSprite2D");
	GD.Print("spriteNode runtime type: ", spriteNode.GetType().Name);

	animatedSprite = spriteNode as AnimatedSprite2D;
	GD.Print("cast worked: ", animatedSprite != null);
	
	}

	public void pickup_power(Power power)
	{
		if (power is ShieldPower)
		{
			shieldPower = (ShieldPower)power;
			shieldPower.ShieldActivate += activateShield;
			shieldPower.activate();
		}

		AddChild(shieldPower);
		power_list.Add(power);
	}

	private void activateShield()
	{
		setShieldPowerActive(true);
	}

	public void TakeDamage(int amount)
	{
		if (!shieldPowerActive)
		{
			healthComponent.reduceHealth(amount);
			GD.Print(healthComponent.getHealth());
			healthUIComponent.changeHealthUI(healthComponent.getHealth());

			if (healthComponent.getHealth() <= 0)
				Die();
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
	
}
