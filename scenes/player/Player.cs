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
    private bool IsTransitioning {get; set;} = false;
    private float TransitionTime {get; set;} = 1.0F;

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
        if (IsTransitioning)
        {
            return;
        }
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
        SetProcess(false);
        IsTransitioning = true;
        Tween tween = CreateTween();
        tween.TweenInterval(TransitionTime);
        tween.TweenCallback(Callable.From(() => GetTree().ReloadCurrentScene()));
    }

    private void CompleteLevel(String nextLevelPath)
    {
        GD.Print("Level Complete!");
        IsTransitioning = true;
        Tween tween = CreateTween();
        tween.TweenInterval(TransitionTime);
        if (nextLevelPath != null)
        {
            tween.TweenCallback(Callable.From(() => GetTree().ChangeSceneToFile(nextLevelPath)));
        }
        else
        {
            tween.TweenCallback(Callable.From(() => GetTree().Quit()));
        }
    }
}
