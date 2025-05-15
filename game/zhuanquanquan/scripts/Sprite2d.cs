using Godot;
using System;

public partial class Sprite2d : Sprite2D
{
    private int _speed = 200;
    private float _angularSpeed = Mathf.Pi;
   [Signal]
    public delegate void HealthChangedEventHandler(int health);
    public override void _Process(double delta)
    {
        //Rotation += _angularSpeed * (float)delta;

        //var velocity = Vector2.Up.Rotated(Rotation) * _speed;
        //Position += velocity * (float)delta;
        // var direction = 0;
        // if (Input.IsActionPressed("ui_left"))
        // {
        //     direction = -1;
        // }
        // if (Input.IsActionPressed("ui_right"))
        // {
        //     direction = 1;
        // }
        // Rotation += _angularSpeed * direction * (float)delta;
        // var velocity = Vector2.Zero;
        // if (Input.IsActionPressed("ui_up"))
        // {
        //     velocity = Vector2.Up.Rotated(Rotation) * _speed;
        // }

        // Position += velocity * (float)delta;

        Rotation += _angularSpeed * (float)delta;
        var velocity = Vector2.Up.Rotated(Rotation) * _speed;
        Position += velocity * (float)delta;
    }


    // We also specified this function name in PascalCase in the editor's connection window.
    private void OnButtonPressed()
    {
        SetProcess(!IsProcessing());
    }

    public override void _Ready()
    {
        var timer = GetNode<Timer>("Timer");
        timer.Timeout += OnTimerTimeout;
    }

    private void OnTimerTimeout()
    {
        Visible = !Visible;
    }
}
