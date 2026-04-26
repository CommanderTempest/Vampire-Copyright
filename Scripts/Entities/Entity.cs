/*
This class is used to derive player and other enemy objects from
*/

using Godot;

public abstract partial class Entity : CharacterBody2D
{
    // some fields to put in here may be speed, and hitbox/hurtbox components
    [Export] public float Speed = 500.0f;

    private HurtboxComponent hurtbox;
    private HitboxComponent hitbox;

    public override void _Ready()
    {
        hurtbox = GetNode<HurtboxComponent>("Hurtbox");
        hitbox = GetNode<HitboxComponent>("Hitbox");
    }
}