using Godot;
using System;

public partial class hit_box : Area2D
{
    [Export] public int damage { get; set; } = 1;

    CollisionShape2D collision;
    Timer disableTimer;

    public override void _Ready()
    {
        collision = GetNode<CollisionShape2D>("CollisionShape2D");
        disableTimer = GetNode<Timer>("DisableHitBoxTimer");
    }

    public void tempdisable(){
        collision.CallDeferred("set", "disabled", true);
        disableTimer.Start();
    }

    public void OnDisableHitBoxTimerTimeout(){
        collision.CallDeferred("set", "disabled", false);

    }
}
