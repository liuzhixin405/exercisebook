using Godot;
using System;

public partial class Main : Node
{
    [Export]
    public PackedScene MobScene { get; set; }

    private int _score;

    public void GameOver()
    {
        GetNode<Timer>("MobTimer").Stop();
        GetNode<Timer>("ScoreTimer").Stop();
        GetNode<Hud>("HUD").ShowGameOver();
         GetNode<AudioStreamPlayer>("Music").Stop();
        GetNode<AudioStreamPlayer>("DeathSound").Play();
    }

    public void NewGame()
    {
        _score = 0;

        var player = GetNode<Player>("Player");
        var startPosition = GetNode<Marker2D>("StartPosition");
        player.Start(startPosition.Position);

        GetNode<Timer>("StartTimer").Start();
        var hud = GetNode<Hud>("HUD");
            hud.UpdateScore(_score);
            hud.ShowMessage("Get Ready!");
            // Note that for calling Godot-provided methods with strings,
            // we have to use the original Godot snake_case name.
            GetTree().CallGroup("mobs", Node.MethodName.QueueFree);
            GetNode<AudioStreamPlayer>("Music").Play();
    }
    // We also specified this function name in PascalCase in the editor's connection window.
    private void OnMobTimerTimeout()
    {
        // Create a new instance of the Mob scene.
        Mob mob = MobScene.Instantiate<Mob>();

        // Choose a random location on Path2D.
        var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
        mobSpawnLocation.ProgressRatio = GD.Randf();

        // 将怪物生成在屏幕上方
        float direction = Mathf.Pi / 2; // 向下方向

        // Set the mob's position to a random location.
        mob.Position = mobSpawnLocation.Position;

        // Add some randomness to the direction.
        direction += (float)GD.RandRange(-Mathf.Pi / 4, Mathf.Pi / 4);
        mob.Rotation = direction;

        // Choose the velocity.
        float speed = (float)GD.RandRange(150.0, 250.0);
        var velocity = new Vector2(0, speed); // 向下移动
        mob.LinearVelocity = velocity.Rotated(direction - Mathf.Pi/2);

        // Spawn the mob by adding it to the Main scene.
        AddChild(mob);
    }
    public override void _Ready()
    {
        MobScene = GD.Load<PackedScene>("res://mob.tscn");
        
        // 连接信号
        GetNode<Timer>("ScoreTimer").Timeout += OnScoreTimerTimeout;
        GetNode<Timer>("StartTimer").Timeout += OnStartTimerTimeout;
        GetNode<Timer>("MobTimer").Timeout += OnMobTimerTimeout;
        GetNode<Player>("Player").Hit += GameOver;
        GetNode<Hud>("HUD").StartGame += NewGame;
        
        NewGame();
    }
    
    private void OnScoreTimerTimeout()
    {
        _score++;
        GetNode<Hud>("HUD").UpdateScore(_score);
    }
    
    private void OnStartTimerTimeout()
    {
        GetNode<Timer>("MobTimer").Start();
        GetNode<Timer>("ScoreTimer").Start();
    }
}
