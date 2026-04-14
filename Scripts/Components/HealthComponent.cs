using System.ComponentModel.DataAnnotations;
using Godot;

public partial class HealthComponent : Node2D
{
    [Signal]
    public delegate void ChangeHealthEventHandler(int hp);

    [Export]
    public int MAX_HEALTH = 10;

    private int current_health;

    public override void _Ready()
    {
        current_health=MAX_HEALTH;
    }

    public void setHealth(int health)
    {
        this.current_health = health;
    }

    public void reduceHealth(int healthToReduceBy)
    {
        this.current_health -= healthToReduceBy;
        EmitSignal(SignalName.ChangeHealth, this.current_health); // Intended to fire so that a UI component picks it up and changes
    }

    public int getHealth()
    {
        return this.current_health;
    }
}