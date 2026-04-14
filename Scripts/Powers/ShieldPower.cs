using Godot;
using System;
using System.ComponentModel;
using System.Net;

public partial class ShieldPower : Power
{
	[Signal]
	public delegate void ShieldActivateEventHandler();

	public const int MAX_COOLDOWN = 15; // in seconds
	private Timer cooldownTimer;

	public override void _Ready()
	{
		cooldownTimer = new Timer();
		cooldownTimer.WaitTime = MAX_COOLDOWN;
		this.AddChild(cooldownTimer);
		//AddUserSignal("ShieldActivate");
		//Activate power
		EmitSignal(SignalName.ShieldActivate);
	}

	// begin cooldown
	public async void shieldDeactivate()
	{
		this.cooldownTimer.WaitTime = MAX_COOLDOWN;
		this.cooldownTimer.Start();
		await ToSignal(cooldownTimer, Timer.SignalName.Timeout);
		EmitSignal(SignalName.ShieldActivate);
	}

	public override void execute_on_pickup()
	{
		//throw new NotImplementedException();
	}

	public override void update()
	{
		//throw new NotImplementedException();
	}

}
