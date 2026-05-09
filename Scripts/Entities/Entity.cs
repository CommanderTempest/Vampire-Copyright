/*
This class is used to derive player and other enemy objects from
*/

using Godot;

public abstract partial class Entity : CharacterBody2D
{
    [Export] public float Speed = 500.0f;

    protected HurtboxComponent hurtbox;
    protected HitboxComponent hitbox;
    protected HealthComponent healthComponent;

    public override void _Ready()
    {
        hurtbox = GetNode<HurtboxComponent>("Hurtbox");
        hitbox = GetNode<HitboxComponent>("Hitbox");

        healthComponent = new HealthComponent();
        this.AddChild(healthComponent);

        if (hurtbox == null)
        {
            GD.PushError(this.Name + " is missing a hurtbox!");
        }
        if (hitbox == null)
        {
            GD.PushError(this.Name + " is missing a hitbox!");
        }

        hitbox.TakeDamage += takeDamage;
    }

    protected virtual void takeDamage(int amount)
    {
        this.healthComponent.reduceHealth(amount);
        if (this.healthComponent.getHealth() <= 0)
        {
            // run death function, enemy will override this
            this.entityDeath();
        }
    }

    protected abstract void entityDeath();
}