using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	[Export] public float Speed = 300.0f;

	public int hp = 5, damage = 1;

	Player player;
	AnimatedSprite2D sprite;
	AudioStreamPlayer snd;

    public override void _Ready()
    {
		player = (Player) GetTree().GetFirstNodeInGroup("player");
		sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }
    public override void _PhysicsProcess(double delta)
	{	
		/*
		Dumb code before i realized i could have just used Vector2.Left * speed
			Vector2 MobPath = new Vector2(ScreenSize.X - ScreenSize.X-50, Position.Y); 
			var direction = GlobalPosition.DirectionTo(MobPath);
			Velocity = direction * Speed;
		*/
		
		Velocity = Vector2.Left * (Speed + (player.time / 4));
		sprite.Play("run");
		MoveAndSlide();
	}

	public void OnHurtBoxHurt(int damage){
		hp -= damage;

		if (hp <= 0){
			player.killCount++;
			snd = GetNode<AudioStreamPlayer>("sndDeath");
			snd.Play();
			QueueFree();
		}
	}

	void OnVisibleOnScreenNotifier2DScreenExited(){
		QueueFree();
	}
}
