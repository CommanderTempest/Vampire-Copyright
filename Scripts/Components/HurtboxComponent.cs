/*
This component is used to signal events to fire when it is interacted with, 
this component is designed to "deal" damage on contact with a Hitbox

Deals damage to Hitbox
*/
using System;
using Godot;

public partial class HurtboxComponent : Area2D
{
    [Export]
    private int damageToDeal = 1;

    public Node owner; // whoever owns this hurtbox component (and cannot be damaged by it)

    public override void _Ready()
    {
        this.AreaEntered += OnHitboxBodyEntered;
    }

    // when this collision shape interacts with another "body"

    private void OnHitboxBodyEntered(Node2D body)
    {
        if (body != owner)
        {
            if (body is HitboxComponent)
            {
                HitboxComponent body2 = body as HitboxComponent; // C# is wild man that I have to do it this way
                // may want to check if body is actually a hitbox component instead
                body2.TakeEntityDamage(this.damageToDeal);
            }
        }
    }
}