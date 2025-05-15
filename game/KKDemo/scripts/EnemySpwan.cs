using Godot;
using System;

public partial class EnemySpwan : Node2D
{
	[Export]
	public PackedScene EnemyPre;
	private float timer = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		timer += (float)delta;
		if(timer >3){
			timer =0;
			var enemy = EnemyPre.Instantiate() as Node2D;
			enemy.Position =this.Position;
			this.GetTree().Root.AddChild(enemy);
		}
	}
}
