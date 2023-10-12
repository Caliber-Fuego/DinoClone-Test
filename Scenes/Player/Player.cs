using Godot;
using System;

public partial class Player : CharacterBody2D
{
	//Stats
	[Export] public float speed {get; set;} = 200;
	public int hp = 1, time = 0, killCount;
	public int score = 0, currentScoreValue = 0, targetScoreValue = 0;
	public Vector2 mov = Vector2.Zero;

	//Nodes
	public Vector2 ScreenSize;
	public CollisionShape2D collision;
	Sprite2D sprite;
	Timer walkTimer;

	//Attacks
	PackedScene swordSlash = (PackedScene) ResourceLoader.Load("res://Scenes/Player/Attack/sword_slash.tscn");

	//Weapons
	//Sword
	int swordLevel = 1;

	//GUI
	Label lblTimer, lblScore, lblScoreResult;
	Panel deathPanel;

    public override void _Ready()
    {
		sprite = GetNode<Sprite2D>("Sprite2D");
		walkTimer = GetNode<Timer>("WalkTimer");
        ScreenSize = GetViewportRect().Size;
		collision = GetNode<CollisionShape2D>("CollisionShape2D");

		//GUI
		lblTimer = GetNode<Label>("CanvasLayer/Control/lblTime");
		lblScore = GetNode<Label>("CanvasLayer/Control/lblScore");
		lblScoreResult =  GetNode<Label>("CanvasLayer/Control/DeathPanel/lblScoreResult");
		deathPanel = GetNode<Panel>("CanvasLayer/Control/DeathPanel");

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
		

			if(walkTimer.IsStopped()){
				if (sprite.Frame >= sprite.Hframes - 1){
					sprite.Frame = 0;
				}else{
					sprite.Frame += 1;
				}
				walkTimer.Start();
			}


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

	public void trackTime(int argtime){
		time = argtime;
		int min, sec;
		min = time/60;
		sec = time % 60;

		string getM = min.ToString("D2");
		string getS = sec.ToString("D2");

		lblTimer.Text = $"{getM}:{getS}";

		var tween = lblScore.CreateTween();

		targetScoreValue = time * killCount;
		tween.TweenMethod(Callable.From<int>(changeScore), score, targetScoreValue, 0.5f);
		score = targetScoreValue;
	}

	public void changeScore(int value){
		String getScore = value.ToString("D12");
		lblScore.Text = $"{getScore}";	
	}
	

	public void death(){
		deathPanel.Visible = true;
		GetTree().Paused = true;

		var tween = deathPanel.CreateTween();
		tween.TweenProperty(deathPanel, "position", new Vector2(414, 130), 1.5f).SetTrans(Tween.TransitionType.Quint).SetEase(Tween.EaseType.In);
		tween.Play();

		String getScore = score.ToString("D12");
		lblScoreResult.Text = $"{getScore}";


		Visible = false;
	}

	public void OnBtnMenuPressed(){
		GetTree().Paused = false;
		var level = GetTree().ChangeSceneToFile("res://Scenes/World/title_screen.tscn");
	}
}