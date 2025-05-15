using Godot;
using System;

public partial class Player : CharacterBody2D
{
	//子弹预设体
	[Export]
	public PackedScene BulletPre;

	//子弹计时器
	private float timer;
	public const float Speed = 300.0f;
	public const float JumpVelocity = -300.0f;

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();

		//角色翻转
		if (direction.X > 0)
		{
			this.GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH = true;
		}
		if (direction.X < 0)
		{
			this.GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH = false;
		}

		if(Velocity.X == 0)
		{
			this.GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("idle");
		}
		else
		{
			this.GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("run");
		}

		timer += (float)delta;
		if(Input.IsKeyLabelPressed(Key.A) && timer > 0.2f)
		{
			timer = 0;
			Bullet bullet = BulletPre.Instantiate<Bullet>();

			float offsetX = this.GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH ? 8 : 50;
			bullet.Position = this.Position +Vector2.Up *-10+ Vector2.Left*offsetX;
			this.GetParent().AddChild(bullet);
		}
	}
}