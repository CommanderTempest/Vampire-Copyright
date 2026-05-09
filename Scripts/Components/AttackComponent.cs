using Godot;
using System;

public partial class AttackComponent : Node
{
	private Node2D attackPivot;
	private Area2D attackArea;
	private Sprite2D slashSprite;
	private Timer attackTimer;

	private CharacterBody2D ownerPlayer;
	private bool attacking = false;

	public override void _Ready()
	{
		GD.Print("AttackComponent ready");

		ownerPlayer = GetParent<CharacterBody2D>();

		attackPivot = ownerPlayer.GetNode<Node2D>("AttackPivot");
		attackArea = ownerPlayer.GetNode<Area2D>("AttackPivot/AttackArea");
		slashSprite = ownerPlayer.GetNode<Sprite2D>("AttackPivot/SlashSprite");
		attackTimer = ownerPlayer.GetNode<Timer>("Timer");

		GD.Print("Pivot found: ", attackPivot != null);
		GD.Print("Area found: ", attackArea != null);
		GD.Print("Sprite found: ", slashSprite != null);
		GD.Print("Timer found: ", attackTimer != null);

		attackArea.Monitoring = false;
		slashSprite.Visible = false;

		attackArea.BodyEntered += OnAttackAreaBodyEntered;
	}

	public async void TryAttack()
	{
		GD.Print("TryAttack called");

		if (attacking)
		{
			GD.Print("Already attacking");
			return;
		}

		attacking = true;

		Vector2 mouseDirection = (ownerPlayer.GetGlobalMousePosition() - ownerPlayer.GlobalPosition).Normalized();
		if (mouseDirection == Vector2.Zero)
			mouseDirection = Vector2.Right;

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

	private void OnAttackAreaBodyEntered(Node body)
	{
		if (body is Enemy)
		{
			Enemy testBody = body as Enemy; //idk why I had to do it this way either
			testBody.die();
		}
	}
}
