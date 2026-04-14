using Godot;
using System;

public abstract partial class Power : Node2D
{
    public bool execute_every_tick = false;   // Whether or not this Power executes every tick

    public abstract void execute_on_pickup(); // Execute whatever this Power does when you obtain it
    public abstract void update();            // Execute every tick
}
