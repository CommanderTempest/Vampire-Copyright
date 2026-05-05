/*
This class is used to derive player and other enemy objects from
*/

using Godot;

public abstract partial class Entity : CharacterBody2D
{
    [Export] public float Speed = 500.0f;

    private HurtboxComponent hurtbox;
    private HitboxComponent hitbox;
    private HealthComponent healthComponent;

    public override void _Ready()
    {
        hurtbox = GetNode<HurtboxComponent>("Hurtbox");
        hitbox = GetNode<HitboxComponent>("Hitbox");

        if (hurtbox == null)
        {
            GD.PushError(this.Name + " is missing a hurtbox!");
        }
        if (hitbox == null)
        {
            GD.PushError(this.Name + " is missing a hitbox!");
        }
    }

    protected void takeDamage(int amount)
    {
        this.healthComponent.reduceHealth(amount);
        if (this.healthComponent.getHealth() <= 0)
        {
            // run death function, enemy will override this
            this.entityDeath();
        }
    }

    protected virtual void entityDeath()
    {
        // die or sumthin
    }
}