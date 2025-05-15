using Godot;
using System;

public partial class Mob : RigidBody2D
{
    public override void _Ready()
    {
        var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        string[] mobTypes = animatedSprite2D.SpriteFrames.GetAnimationNames();
        animatedSprite2D.Play(mobTypes[GD.Randi() % mobTypes.Length]);
    }
    // We also specified this function name in PascalCase in the editor's connection window.
    private void OnVisibleOnScreenNotifier2DScreenExited()
    {
        QueueFree();
    }
}
