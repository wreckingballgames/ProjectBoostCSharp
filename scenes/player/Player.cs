using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class Player : RigidBody3D
{
    [Export]
    private float TorqueThrust {get; set;} = 100.0F;
    [Export(PropertyHint.Range, "750.0F, 3000.0F")]
    private float Thrust {get; set;} = 1000.0F;

    private string NextLevelPath {get; set;}
    private bool IsTransitioning {get; set;} = false;
    private float TransitionTime {get; set;} = 1.5F;

    private AudioStreamPlayer ExplosionSFX {get; set;}
    private AudioStreamPlayer SuccessSFX {get; set;}
    private AudioStreamPlayer3D RocketSFX {get; set;}

    private GpuParticles3D BoosterParticlesL {get; set;}
    private GpuParticles3D BoosterParticlesC {get; set;}
    private GpuParticles3D BoosterParticlesR {get; set;}

    private GpuParticles3D ExplosionParticles {get; set;}
    private GpuParticles3D SuccessParticles {get; set;}

    public override void _Ready()
    {
        // Connect Signals
        BodyEntered += (Node body) => OnBodyEntered(body);

        // Get References to Children
        ExplosionSFX = GetNode<AudioStreamPlayer>("%ExplosionSFX");
        ExplosionParticles = GetNode<GpuParticles3D>("%ExplosionParticles");
        SuccessSFX = GetNode<AudioStreamPlayer>("%SuccessSFX");
        SuccessParticles = GetNode<GpuParticles3D>("%SuccessParticles");
        RocketSFX = GetNode<AudioStreamPlayer3D>("%RocketSFX");
        BoosterParticlesL = GetNode<GpuParticles3D>("%BoosterParticlesL");
        BoosterParticlesC = GetNode<GpuParticles3D>("%BoosterParticlesC");
        BoosterParticlesR = GetNode<GpuParticles3D>("%BoosterParticlesR");

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
        ExplosionParticles.Emitting = true;
        
        Tween tween = BeginTransition();
        tween.TweenCallback(Callable.From(() => GetTree().ReloadCurrentScene()));
    }

    private void CompleteLevel(String nextLevelPath)
    {
        // TODO: Add UI popup for completing level (one for each condition)
        GD.Print("Level Complete!");
        SuccessSFX.Play();
        SuccessParticles.Emitting = true;

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
            BoosterParticlesC.Emitting = true;
        }
        else
        {
            BoosterParticlesC.Emitting = false;
        }

        float direction = Input.GetAxis("rotate_right", "rotate_left");
        HandleBoosterParticles(direction);
        ApplyTorque(new(0.0F, 0.0F, direction * TorqueThrust * delta));
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

    private void HandleBoosterParticles(float direction)
    {
        // Left/right can seem mixed up because of how rotating the ship works
        if (direction < 0)
        {
            BoosterParticlesL.Emitting = false;
            BoosterParticlesR.Emitting = true;
        }
        else if (direction > 0)
        {
            BoosterParticlesL.Emitting = true;
            BoosterParticlesR.Emitting = false;
        }
        else
        {
            BoosterParticlesL.Emitting = false;
            BoosterParticlesR.Emitting = false;
        }
    }
}
