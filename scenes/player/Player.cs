using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Player : Node3D
{
    private float RotationSpeed {get; set;} = 0.05F;

    public override void _Process(double delta)
    {
        if (Input.IsActionPressed("boost"))
        {
            float y = Position.Y + (float)delta;
            Position = Position with {Y = y};
        }

        if (Input.IsActionPressed("rotate_left"))
        {
            RotateZ(RotationSpeed);
        }
        else if (Input.IsActionPressed("rotate_right"))
        {
            RotateZ(-RotationSpeed);
        }

        base._Process(delta);
    }
}
