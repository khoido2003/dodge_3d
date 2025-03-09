using System;
using Godot;

public partial class Mob : CharacterBody3D
{
    [Signal]
    public delegate void SquashedEventHandler();

    [Export]
    public int MinSpeed { get; set; } = 10;

    [Export]
    public int MaxSpeed { get; set; } = 18;

    public void Squash()
    {
        EmitSignal(SignalName.Squashed);
        QueueFree();
    }

    public override void _PhysicsProcess(double delta)
    {
        MoveAndSlide();
    }

    public void Initialize(Vector3 startPosition, Vector3 playerPosition)
    {
        // We position the mob by placing it at the startPosition
        // and rotate it towards playerPostion so it looks at the player
        LookAtFromPosition(startPosition, playerPosition, Vector3.Up);

        // Rotate this mob randomly within range of -45 and 45 degrees,
        // so that it doesn't move directly to the player
        RotateY((float)GD.RandRange(-Math.PI / 4.0, Math.PI / 4.0));

        // Random speed
        int randomSpeed = GD.RandRange(MinSpeed, MaxSpeed);

        // Calculate a forward velocity that represent the speed
        Velocity = Vector3.Forward * randomSpeed;

        // Rotate the velocity vector based on the mob's Y Rotation
        // in order to move in the direction the mob is looking
        Velocity = Velocity.Rotated(Vector3.Up, Rotation.Y);
    }

    private void OnVisibilityNotifierScreenExited()
    {
        QueueFree();
    }
}
