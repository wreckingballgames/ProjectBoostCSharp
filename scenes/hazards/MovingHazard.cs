using Godot;
using System;

public partial class MovingHazard : AnimatableBody3D
{
    [Export]
    private Vector3 Destination {get; set;} = new(0, 0, 0);
    [Export]
    private float TravelDuration {get; set;} = 1.0F;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        StartMoving();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    private void StartMoving()
    {
        Tween tween = CreateTween();
        // Set tween to loop infinitely; note that I use Node.CreateTween() so tweens live just as long as the node they are bound to
        tween.SetLoops();
        tween.SetTrans(Tween.TransitionType.Cubic);
        tween.TweenProperty(this, "global_position", GlobalPosition + Destination, TravelDuration);
        tween.TweenProperty(this, "global_position", GlobalPosition, TravelDuration);
    }
}
