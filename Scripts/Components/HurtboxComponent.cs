/*
This component is used to signal events to fire when it is interacted with, this component is designed to "deal" damage on contact with a Hitbox
*/
using Godot;

public partial class HurtboxComponent : Area2D
{
    public Node owner; // whoever owns this hurtbox component (and cannot be damaged by it)

    // when this collision shape interacts with another "body"
    public void OnHitboxBodyEntered(HitboxComponent body)
    {
        if (body != owner)
        {
            // may want to check if body is actually a hitbox component instead
            body.TakeEntityDamage(1);
        }
    }
}