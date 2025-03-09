using System;
using Godot;

public partial class Player : CharacterBody3D
{
    [Signal]
    public delegate void HitEventHandler();

    // How fast the player moves in meters per second.
    [Export]
    public int Speed { get; set; } = 14;

    // The downward acceleration when in the air, in meters per second squared.
    [Export]
    public int FallAcceleration { get; set; } = 75;

    [Export]
    public int JumpImpulse { get; set; } = 20;

    [Export]
    public int BounceImpulse { get; set; } = 16;

    private Vector3 _targetVelocity = Vector3.Zero;

    public override void _PhysicsProcess(double delta)
    {
        // We create a local variable to store the input direction.
        var direction = Vector3.Zero;

        // We check for each move input and update the direction accordingly.
        if (Input.IsActionPressed("move_right"))
        {
            direction.X += 1.0f;
        }
        if (Input.IsActionPressed("move_left"))
        {
            direction.X -= 1.0f;
        }
        if (Input.IsActionPressed("move_back"))
        {
            // Notice how we are working with the vector's X and Z axes.
            // In 3D, the XZ plane is the ground plane.
            direction.Z += 1.0f;
        }
        if (Input.IsActionPressed("move_forward"))
        {
            direction.Z -= 1.0f;
        }

        if (IsOnFloor() && Input.IsActionPressed("jump"))
        {
            _targetVelocity.Y = JumpImpulse;
        }

        if (direction != Vector3.Zero)
        {
            direction = direction.Normalized();
            // Setting the basis property will affect the rotation of the node.
            GetNode<Node3D>("Pivot").Basis = Basis.LookingAt(direction);
        }

        // Ground velocity
        _targetVelocity.X = direction.X * Speed;
        _targetVelocity.Z = direction.Z * Speed;

        // Vertical velocity
        if (!IsOnFloor()) // If in the air, fall towards the floor. Literally gravity
        {
            _targetVelocity.Y -= FallAcceleration * (float)delta;
        }

        // Moving the character
        Velocity = _targetVelocity;

        // Iterate through all collisions that occured this frame
        for (int index = 0; index < GetSlideCollisionCount(); index++)
        {
            // Get one of the collison with the player
            KinematicCollision3D collision = GetSlideCollision(index);

            // If the collision is with a mob
            if (collision.GetCollider() is Mob mob)
            {
                if (Vector3.Up.Dot(collision.GetNormal()) > 0.1f)
                {
                    mob.Squash();
                    _targetVelocity.Y = BounceImpulse;

                    // Prevent duplicate call
                    break;
                }
            }
        }

        MoveAndSlide();
    }

    private void Die()
    {
        EmitSignal(SignalName.Hit);
        QueueFree();
    }

    private void OnMobDetectorBodyEntered(Node3D body)
    {
        Die();
    }
}
