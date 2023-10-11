using Godot;
using System;

public partial class Player : CharacterBody2D
{
	//Stats
	[Export] public float speed {get; set;} = 200;
	public int hp = 1;
	public Vector2 mov = Vector2.Zero;

	//Nodes
	public Vector2 ScreenSize;
	public CollisionShape2D collision;

	//Attacks
	PackedScene swordSlash = (PackedScene) ResourceLoader.Load("res://Scenes/Player/Attack/sword_slash.tscn");

	//Weapons
	//Sword
	int swordLevel = 1;

    public override void _Ready()
    {
        ScreenSize = GetViewportRect().Size;
		collision = GetNode<CollisionShape2D>("CollisionShape2D");
		GD.Print(ScreenSize.X + ","+ScreenSize.Y);

    }


    public override void _PhysicsProcess(double delta)
    {
		movement();

		if(Input.IsActionJustPressed("LeftClick")){
			attack();
		}
	}

	public void movement(){
		var x_mov = 0;
		var y_mov = Input.GetActionStrength("MoveDown") - Input.GetActionStrength("MoveUp");
		var mov = new Vector2(x:x_mov, y:y_mov);
		
		Velocity = mov.Normalized() * speed;
		MoveAndSlide();


		float Radius = collision.Shape.GetRect().Size.X /2;
    	Vector2 NewPosition = new Vector2();

    	if(Position.Y + Radius >= ScreenSize.Y - 5|| Position.Y - Radius <= 400)
    	{
        	NewPosition.X = Mathf.Clamp(Position.X, 0 + Radius, ScreenSize.X - Radius);
        	NewPosition.Y = Mathf.Clamp(Position.Y, 400 + Radius, ScreenSize.Y - Radius - 5); 
        	Position = NewPosition;
    	}
	}

	public void attack(){

		if(swordLevel > 0){
			SwordAttack();
		}
	}

	public void SwordAttack(){
		sword_slash swordAttack = (sword_slash) swordSlash.Instantiate();
		swordAttack.Position = Position;
		swordAttack.level = swordLevel;
		GetParent().AddChild(swordAttack);
	}

	public void OnHurtBoxHurt(int damage){
		GD.Print("player damaged!");
		hp -= damage;

		if (hp <= 0){
			death();
		}
	}

	public void death(){
		GD.Print("you died!");
		Visible = false;
	}
}