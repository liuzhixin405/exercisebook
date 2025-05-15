using Godot;
using System;

public partial class Enemy : AnimatedSprite2D
{
	Vector2 dir;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//朝向玩家的方向向量
		dir = GetTree().Root.GetNode("Node2D").GetNode<Node2D>("player").Position - this.Position;
		var  area = this.GetNode<Area2D>("Area2D");
		area.Connect("area_entered",new Callable(this,"OnCollisionEnter"));
	}
	void OnCollisionEnter(Area2D area)
	{
		var go = area.GetParent() as Node2D;

		if(go.IsInGroup("bullets"))
		{
			go.QueueFree();
			this.QueueFree();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		this.Position +=dir.Normalized()*100 *(float)delta;
	}
}
