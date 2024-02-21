using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Player : RigidBody3D
{
    [Export]
    private float TorqueThrust {get; set;} = 100.0F;
    [Export(PropertyHint.Range, "750.0F, 3000.0F")]
    private float Thrust {get; set;} = 1000.0F;

    private String NextLevelPath {get; set;}

    public override void _Ready()
    {
        BodyEntered += (Node body) => OnBodyEntered(body);

        base._Ready();
    }

    public override void _Process(double delta)
    {
        float deltaAsFloat = (float)delta;

        if (Input.IsActionPressed("boost"))
        {
            ApplyCentralForce(Basis.Y * deltaAsFloat * Thrust);;
        }

        if (Input.IsActionPressed("rotate_left"))
        {
            ApplyTorque(new(0.0F, 0.0F, TorqueThrust * deltaAsFloat));
        }
        else if (Input.IsActionPressed("rotate_right"))
        {
            ApplyTorque(new(0.0F, 0.0F, -TorqueThrust * deltaAsFloat));
        }

        base._Process(delta);
    }

    private void OnBodyEntered(Node body)
    {
        if (body.IsInGroup("hazard"))
        {
            CrashSequence();
        }
        else if (body.IsInGroup("goal"))
        {
            LandingPad goal = body as LandingPad;
            CompleteLevel(goal?.NextLevelPath);
        }
    }

    private void CrashSequence()
    {
        GD.Print("KABLOOEY");
        GetTree().ReloadCurrentScene();
    }

    private void CompleteLevel(String nextLevelPath)
    {
        GD.Print("Level Complete!");
        if (nextLevelPath != null)
        {
            GetTree().CallDeferred("change_scene_to_file", nextLevelPath);
        }
        else
        {
            GetTree().Quit();
        }
    }
}
