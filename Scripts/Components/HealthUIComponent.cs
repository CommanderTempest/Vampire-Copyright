using System;
using Godot;

public partial class HealthUiComponent : Node2D
{
	[Export]
	public TextureProgressBar healthBarUI;

	private Tween tween;

	public override void _Ready()
	{
		healthBarUI = new TextureProgressBar();
		this.healthBarUI.TextureUnder = GD.Load<Texture2D>("Assets/HealthBarEmpty32.png");
		this.healthBarUI.TextureProgress = GD.Load<Texture2D>("Assets/HealthBarFull32.png");
		this.AddChild(healthBarUI);
	}

	public void SetMaxHealth(int max_health)
	{
		this.healthBarUI.MaxValue = max_health;
	}

	public void changeHealthUI(int health)
	{
		// remove tween if it's already animating
		if (tween != null)
		{
			tween.Kill();
		}
		tween = GetTree().CreateTween();
		tween.TweenProperty(healthBarUI, "value", health, 0.5f);
	}
}
