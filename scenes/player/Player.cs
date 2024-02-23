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
    private float TransitionTime {get; set;} = 1.5F;

    private AudioStreamPlayer ExplosionSFX {get; set;}
    private AudioStreamPlayer SuccessSFX {get; set;}
    private AudioStreamPlayer3D RocketSFX {get; set;}

    public override void _Ready()
    {
        // Connect Signals
        BodyEntered += (Node body) => OnBodyEntered(body);

        // Get References to Children
        ExplosionSFX = GetNode<AudioStreamPlayer>("%ExplosionSFX");
        SuccessSFX = GetNode<AudioStreamPlayer>("%SuccessSFX");
        RocketSFX = GetNode<AudioStreamPlayer3D>("%RocketSFX");

        base._Ready();
    }

    public override void _Process(double delta)
    {
        HandleInput((float)delta);
        HandleRocketSFX();

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
        // TODO: Add UI popup for crashing
        GD.Print("KABLOOEY");
        ExplosionSFX.Play();
        
        Tween tween = BeginTransition();
        tween.TweenCallback(Callable.From(() => GetTree().ReloadCurrentScene()));
    }

    private void CompleteLevel(String nextLevelPath)
    {
        // TODO: Add UI popup for completing level (one for each condition)
        GD.Print("Level Complete!");
        SuccessSFX.Play();

        Tween tween = BeginTransition();
        if (nextLevelPath != null)
        {
            tween.TweenCallback(Callable.From(() => GetTree().ChangeSceneToFile(nextLevelPath)));
        }
        else
        {
            tween.TweenCallback(Callable.From(() => GetTree().Quit()));
        }
    }

    private Tween BeginTransition()
    {
        // FIXME
        // Is this nasty function with side effects and a messy object creation
        //   better than repeating three lines of code in two places?
        // In this case, I think the function is better. This transition process
        //   necessarily always leads to resetting the scene, dodging the
        //   potential consequences of my ignorance of where this tween lives.
        SetProcess(false);
        Tween tween = CreateTween();
        tween.BindNode(this);
        tween.TweenInterval(TransitionTime);
        IsTransitioning = true;
        return tween;
    }

    private void HandleInput(float delta)
    {
        if (Input.IsActionPressed("boost"))
        {
            ApplyCentralForce(Basis.Y * delta * Thrust);
        }

        if (Input.IsActionPressed("rotate_left"))
        {
            ApplyTorque(new(0.0F, 0.0F, TorqueThrust * delta));
        }
        else if (Input.IsActionPressed("rotate_right"))
        {
            ApplyTorque(new(0.0F, 0.0F, -TorqueThrust * delta));
        }
    }

    private void HandleRocketSFX()
    {
        if (Input.IsActionJustPressed("boost"))
        {
            RocketSFX.Play();
        }
        else if (Input.IsActionJustReleased("boost"))
        {
            RocketSFX.Stop();
        }
    }
}
