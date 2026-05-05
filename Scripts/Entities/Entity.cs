/*
This class is used to derive player and other enemy objects from
*/

using Godot;

public abstract partial class Entity : CharacterBody2D
{
    [Export] public float Speed = 500.0f;

    private HurtboxComponent hurtbox;
    private HitboxComponent hitbox;

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
}