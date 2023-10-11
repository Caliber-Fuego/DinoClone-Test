using Godot;
using System;

public partial class Background : ParallaxBackground
{
	[Export]
	public int ScrollSpeed = 500;

    public override void _PhysicsProcess(double delta)
    {
         ScrollBaseOffset -= new Vector2(ScrollSpeed, 0) * (float) delta;
    }
}
