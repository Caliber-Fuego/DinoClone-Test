using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	[Export] public float Speed = 300.0f;

	public int hp = 5, damage = 1;


    public override void _Ready()
    {
    }
    public override void _PhysicsProcess(double delta)
	{	
		/*
		Dumb code before i realized i could have just used Vector2.Left * speed
			Vector2 MobPath = new Vector2(ScreenSize.X - ScreenSize.X-50, Position.Y); 
			var direction = GlobalPosition.DirectionTo(MobPath);
			Velocity = direction * Speed;
		*/
		
		Velocity = Vector2.Left * Speed;

		MoveAndSlide();
	}

	public void OnHurtBoxHurt(int damage){
		GD.Print("enemy damaged!");
		hp -= damage;

		if (hp <= 0){
			QueueFree();
		}
	}

	void OnVisibleOnScreenNotifier2DScreenExited(){
		QueueFree();
	}
}
