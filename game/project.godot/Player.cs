using Godot;
using System;
using System.Diagnostics;

public partial class Player : Area2D
{
    [Export]
    public int Speed = 400;
    public Vector2 ScreenSize;

    [Signal]
    public delegate void HitEventHandler();
    public override void _Ready()
    {
        Debug.WriteLine("Player _Ready");
        ScreenSize = GetViewportRect().Size;
        Hide();
    }
    public override void _Process(double delta)
    {
        var velocity = Vector2.Zero; // The player's movement vector.

        if (Input.IsActionPressed("move_right"))
        {
            velocity.X += 1;
        }

        if (Input.IsActionPressed("move_left"))
        {
            velocity.X -= 1;
        }

        var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

        if (velocity.Length() > 0)
        {
            velocity = velocity.Normalized() * Speed;
            animatedSprite2D.Play();
        }
        else
        {
            animatedSprite2D.Stop();
        }

        Position += velocity * (float)delta;
        Position = new Vector2(
            Mathf.Clamp(Position.X, 0, ScreenSize.X),
            Mathf.Clamp(Position.Y, 0, ScreenSize.Y)
        );
       
        if(velocity.X != 0)
        {
            animatedSprite2D.Animation = "walk";
            animatedSprite2D.FlipV = false;
            animatedSprite2D.FlipH = velocity.X < 0;
        }
    }
    // We also specified this function name in PascalCase in the editor's connection window.
    private void OnBodyEntered(Node2D body)
    {
        Hide(); // Player disappears after being hit.
        EmitSignal(SignalName.Hit);
        // Must be deferred as we can't change physics properties on a physics callback.
        GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
    }
    public void Start(Vector2 position)
    {
        // 只使用传入位置的X坐标，Y坐标固定
        Position = new Vector2(position.X, position.Y);
        Show();
        GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
    }
}
