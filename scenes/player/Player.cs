using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Player : RigidBody3D
{
    private float RotationSpeed {get; set;} = 100F;

    public override void _Process(double delta)
    {
        float deltaAsFloat = (float)delta;

        if (Input.IsActionPressed("boost"))
        {
            ApplyCentralForce(Godot.Vector3.Up * deltaAsFloat * 1000.0F);;
        }

        if (Input.IsActionPressed("rotate_left"))
        {
            ApplyTorque(new(0.0F, 0.0F, RotationSpeed * deltaAsFloat));
        }
        else if (Input.IsActionPressed("rotate_right"))
        {
            ApplyTorque(new(0.0F, 0.0F, -RotationSpeed * deltaAsFloat));
        }

        base._Process(delta);
    }
}
