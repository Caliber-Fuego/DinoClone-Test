using Godot;
using System;

public partial class sword_slash : Area2D
{
    public int level = 1, hitAmt = 1, speed = 1000, damage = 5;

    public override void _Ready()
    {
        switch(level){
            case 1:
            hitAmt = 1;
            speed = 1000;
            break;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Position += Vector2.Right * speed * (float) delta;
    }

    public void enemyHit(){
        hitAmt --;
        if (hitAmt <= 0){
            QueueFree();
        }
    }

    void OnVisibleOnScreenNotifier2DScreenExited(){
		QueueFree();
	}
}
