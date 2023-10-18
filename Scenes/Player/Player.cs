using Godot;
using System;
using System.Data.SQLite;

public partial class Player : CharacterBody2D
{
	//Stats
	[Export] public float speed {get; set;} = 200;
	public int hp = 1, time = 0, killCount;
	public int score = 0, currentScoreValue = 0, targetScoreValue = 0;
	public string userName;
	public Vector2 mov = Vector2.Zero;
	Global global;

	//Nodes
	public Vector2 ScreenSize;
	public CollisionShape2D collision;
	AnimatedSprite2D playerSprite;
	string spritePath;
	Sprite2D sprite;
	Timer walkTimer;
	Enemy enemy;
	Leaderboards leaderboardstest;

	//Database
	public string dataPath {get;} = "C:/Users/Asus/Documents/School Documents/Object Oriented Programming/Tests/DinoClone Test/Database/data.db";
	SQLiteConnection dbConnection;
	SQLiteDataReader dbRead;
	SQLiteCommand dbCmd;

	//Attacks
	PackedScene swordSlash = (PackedScene) ResourceLoader.Load("res://Scenes/Player/Attack/sword_slash.tscn");

	//Weapons
	//Sword
	int swordLevel = 1;

	//GUI
	Label lblTimer, lblScore, lblScoreResult, lblUserScoreResult, lblScoreLabel;
	GridContainer leaderboards, userBoard;
	Button scoreBtn, submitBtn;
	Panel deathPanel, leaderboardsPanel;
	VBoxContainer placementVbox, nameVbox, scoreVbox;
	VBoxContainer userplaceVbox, usernameVbox, userscoreVbox;
	Font font;

    public override void _Ready()
    {
		global = GetNode<Global>("/root/Global");

		walkTimer = GetNode<Timer>("WalkTimer");
        ScreenSize = GetViewportRect().Size;
		collision = GetNode<CollisionShape2D>("CollisionShape2D");

		
		//Sprite
		spritePath = global.spritePath;
		sprite = GetNode<Sprite2D>("Sprite2D");
		playerSprite = GetNode<AnimatedSprite2D>(spritePath);
		playerSprite.Visible = true;
		playerSprite.Play("run");

		//GUI
		lblTimer = GetNode<Label>("CanvasLayer/Control/lblTime");
		lblScore = GetNode<Label>("CanvasLayer/Control/lblScore");
		lblScoreLabel = GetNode<Label>("CanvasLayer/Control/DeathPanel/lblName");
		lblScoreResult =  GetNode<Label>("CanvasLayer/Control/DeathPanel/lblScoreResult");
		lblUserScoreResult = GetNode<Label>("CanvasLayer/Control/DeathPanel/lblUserResult");
		deathPanel = GetNode<Panel>("CanvasLayer/Control/DeathPanel");
		leaderboardsPanel = GetNode<Panel>("CanvasLayer/Control/LeaderboardsPanel");

		leaderboards = GetNode<GridContainer>("CanvasLayer/Control/DeathPanel/ScrollContainer/Leaderboards");
		userBoard = GetNode<GridContainer>("CanvasLayer/Control/DeathPanel/userScoreboard");
		placementVbox = GetNode<VBoxContainer>("CanvasLayer/Control/DeathPanel/ScrollContainer/Leaderboards/placeContainer");
		nameVbox 	  = GetNode<VBoxContainer>("CanvasLayer/Control/DeathPanel/ScrollContainer/Leaderboards/nameContainer");
		scoreVbox 	  = GetNode<VBoxContainer>("CanvasLayer/Control/DeathPanel/ScrollContainer/Leaderboards/scoreContainer");
		userplaceVbox = GetNode<VBoxContainer>("CanvasLayer/Control/DeathPanel/userScoreboard/userPlaceContainer");
		usernameVbox 	  = GetNode<VBoxContainer>("CanvasLayer/Control/DeathPanel/userScoreboard/userNameContainer");
		userscoreVbox 	  = GetNode<VBoxContainer>("CanvasLayer/Control/DeathPanel/userScoreboard/userScoreContainer");;

		scoreBtn = GetNode<Button>("CanvasLayer/Control/DeathPanel/ScoreBtn");
		submitBtn = GetNode<Button>("CanvasLayer/Control/DeathPanel/SubmitBtn");
		

		//Database Related
		dbConnection = new SQLiteConnection();
		userName = global.userName;

    }


    public override void _PhysicsProcess(double delta)
    {
		movement();

		if(Input.IsActionJustPressed("LeftClick")){
			playerSprite.Play("slash");
			attack();
		}
	}

	public void movement(){
		var x_mov = 0;
		var y_mov = Input.GetActionStrength("MoveDown") - Input.GetActionStrength("MoveUp");
		var mov = new Vector2(x:x_mov, y:y_mov);
		
			/*
			if(walkTimer.IsStopped()){
				if (sprite.Frame >= sprite.Hframes - 1){
					sprite.Frame = 0;
				}else{
					sprite.Frame += 1;
				}
				walkTimer.Start();
			}*/

		if (Input.IsActionPressed("accelerate")){
			Velocity = mov.Normalized() * (speed * 2.5f);
		}else{
			Velocity = mov.Normalized() * speed;
		}

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
		hp -= damage;

		if (hp <= 0){
			death();
		}
	}

	public void OnAnimationFinished(){
		playerSprite.Play("run");
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

		leaderboardsPanel.Position = new Vector2(260, 130);
		String getScore = score.ToString("D12");
		lblScoreResult.Text = $"{getScore}";


		Visible = false;
	}

	public void OnBtnMenuPressed(){
		GetTree().Paused = false;
		var level = GetTree().ChangeSceneToFile("res://Scenes/World/title_screen.tscn");
	}

	public void OnLeaderboardsMenuPressed(){
		listLeaderboards();
		listUserScore();
		lblScoreLabel.Visible = false;
		lblScoreResult.Visible = false;
		scoreBtn.Visible = false;

		submitBtn.Visible = true;
		lblUserScoreResult.Visible = true;
		leaderboards.Visible = true;
	}

	public void OnSubmitScoreMenuPressed(){
		GetTree().Paused = false;
		submitUserScore();
		var level = GetTree().ChangeSceneToFile("res://Scenes/World/title_screen.tscn");
	}

	void listLeaderboards(){
		dbConnection = new SQLiteConnection($"Data Source= {dataPath}");
		dbConnection.Open();
		string query = "SELECT (SELECT COUNT(DISTINCT Score) FROM OverallScores WHERE Score >= t.Score) AS placement, Name, Score FROM OverallScores t ORDER BY Score DESC LIMIT 10";
		dbCmd = new SQLiteCommand(query, dbConnection);
		dbRead = dbCmd.ExecuteReader();

		int numberOfRows = 10; // Set the number of rows you want to display
		Label[] placementLabels = new Label[numberOfRows];
		Label[] nameLabels = new Label[numberOfRows];
		Label[] scoreLabels = new Label[numberOfRows];

		for (int i = 0; i < numberOfRows; i++)
		{
    		placementLabels[i] = new Label();
    		nameLabels[i] = new Label();
    		scoreLabels[i] = new Label();
		}

		int row = 0;

		if (dbRead.HasRows)
		{
    		while (dbRead.Read() && row < numberOfRows)
   	 		{
        	int placement = dbRead.GetInt32(dbRead.GetOrdinal("placement"));
        	string name = dbRead.GetString(dbRead.GetOrdinal("Name"));
        	int score = dbRead.GetInt32(dbRead.GetOrdinal("Score"));

        	placementLabels[row].Text = placement.ToString();
        	nameLabels[row].Text = name;
        	scoreLabels[row].Text = score.ToString("D12");

        	row++;
    		}
		}
		
		
		for (int i = 0; i < numberOfRows; i++)
		{
    		placementVbox.AddChild(placementLabels[i]);
    		nameVbox.AddChild(nameLabels[i]);
    		scoreVbox.AddChild(scoreLabels[i]);
		}

		dbRead.Close();
		dbConnection.Close();
	}

	void listUserScore(){
		dbConnection = new SQLiteConnection($"Data Source= {dataPath}");
		dbConnection.Open();
		string query = $"WITH NewScore AS ( SELECT '{userName}' AS Name, {score} AS Score)"+ 
			           "SELECT (SELECT COUNT(DISTINCT Score) FROM OverallScores WHERE Score >= ns.Score) + 1 AS placement,"+
					   "ns.Name, ns.Score "+
					   "FROM NewScore ns";
		dbCmd = new SQLiteCommand(query, dbConnection);
		dbRead = dbCmd.ExecuteReader();

		Label placementLabels = new Label();
		Label nameLabels = new Label();
		Label scoreLabels = new Label();

		while (dbRead.Read())
   	 		{
        	int placement = dbRead.GetInt32(dbRead.GetOrdinal("placement"));
        	string name = dbRead.GetString(dbRead.GetOrdinal("Name"));
        	int score = dbRead.GetInt32(dbRead.GetOrdinal("Score"));

        	placementLabels.Text = placement.ToString();
        	nameLabels.Text = name;
        	scoreLabels.Text = score.ToString("D12");
    		}
		
		userplaceVbox.AddChild(placementLabels);
		usernameVbox.AddChild(nameLabels);
		userscoreVbox.AddChild(scoreLabels);

		dbRead.Close();
		dbConnection.Close();
	}

	void submitUserScore(){
		dbConnection = new SQLiteConnection($"Data Source= {dataPath}");
		dbConnection.Open();
		string query = "INSERT INTO OverallScores (Name, Score) VALUES (@Value1, @Value2);";
		dbCmd = new SQLiteCommand(query, dbConnection);

		dbCmd.Parameters.AddWithValue("@Value1", userName);
		dbCmd.Parameters.AddWithValue("@Value2", score);

		dbCmd.ExecuteNonQuery();
		
		GD.Print("Score added!");
		dbCmd.Dispose();
		dbConnection.Close();
	}


	void printData(){
		dbConnection = new SQLiteConnection($"Data Source= {dataPath}");
		dbConnection.Open();
		string query = "select * from OverallScores";
		dbCmd = new SQLiteCommand(query, dbConnection);
		dbRead = dbCmd.ExecuteReader();

		if(dbRead.HasRows){
			while (dbRead.Read()){
				int playerID = dbRead.GetInt32(dbRead.GetOrdinal("ID"));
				String playerName = dbRead.GetString(dbRead.GetOrdinal("Name"));
				int playerScore = dbRead.GetInt32(dbRead.GetOrdinal("Score"));
				GD.Print($"ID: {playerID} Name: {playerName} Score {playerScore}");
			}
		}

		dbRead.Close();
		dbConnection.Close();

	}
}