/*
This class is responsible for detecting collisions
*/

using Godot;

public partial class HitboxComponent : Area2D
{
    [Signal]
	public delegate void TakeDamageEventHandler(int amount);

    public void TakeDamage(int damageToTake)
    {
        // signal to entity to reduce hp
        EmitSignal(SignalName.TakeDamage, damageToTake);
    }
}