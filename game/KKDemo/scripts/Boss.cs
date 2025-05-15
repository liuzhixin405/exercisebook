using Godot;
using System;

public partial class Boss : AnimatedSprite2D
{
	private float timer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var area = this.GetNode<Area2D>("Area2D");
		area.Connect("area_entered", new Callable(this, nameof(OnCollisionEntered)));

		this.Connect("frame_changed",new Callable(this,nameof(OnFrameChanged)));
	}

    void OnFrameChanged()
    {
        if(timer >0)
		{
			if(this.Frame ==0||this.Frame==3)
			{
				this.Position +=Vector2.Up *20;
			}
		}else{
			if(this.Frame ==0||this.Frame==3)
			this.Position -=Vector2.Up * 20;
		}
    }

    void OnCollisionEntered(Area2D area)
	{
		var go = area.GetParent() as Node2D;

		if(go.IsInGroup("bullets"))
		{
			go.QueueFree();
			this.Play("up");
			timer = 1;
		}
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		timer -= (float)delta;
		if(timer  <0 )
		{
			this .Play("down");
		}
	}
	
}
