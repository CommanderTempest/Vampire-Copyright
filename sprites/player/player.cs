using System;
using System.Collections;
using System.Collections.Generic;
using Godot;

public partial class player : CharacterBody2D
{
	[Export] public float Speed = 500.0f;

	private ArrayList power_list;

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
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Velocity = direction * Speed;
		MoveAndSlide();
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
