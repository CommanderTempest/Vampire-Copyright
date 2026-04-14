using System.ComponentModel.DataAnnotations;
using Godot;

public partial class HealthComponent : Node
{
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
    }

    public int getHealth()
    {
        return this.current_health;
    }
}