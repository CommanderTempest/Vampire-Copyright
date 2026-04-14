using Godot;

public partial class HealthUIComponent : Node
{
    public TextureProgressBar healthBarUI;

    public override void _Ready()
    {
        healthBarUI = new TextureProgressBar();
    }

    public void changeHealthUI(int health)
    {
        Tween tween = new Tween();
        tween.TweenProperty(healthBarUI, "value", health, 0.5f);
    }
}