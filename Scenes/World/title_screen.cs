using Godot;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

public partial class title_screen : Control
{
	string playLevel = "res://Scenes/World/main.tscn";

	//Database
	public string dataPath {get;} = "C:/Users/Asus/Documents/School Documents/Object Oriented Programming/Tests/DinoClone Test/Database/data.db";
	SQLiteConnection dbConnection;
	SQLiteDataReader dbRead;
	SQLiteCommand dbCmd;

	//GUI
	Button scoreBtn, returnBtn, playBtn;
	ScrollContainer scrollContainer;
	GridContainer leaderboards;
	VBoxContainer placementVbox, nameVbox, scoreVbox;

    public override void _Ready()
    {
        //Database Related
		dbConnection = new SQLiteConnection();

		//GUI
		scrollContainer = GetNode<ScrollContainer>("CanvasLayer/ScrollContainer");
		leaderboards = GetNode<GridContainer>("CanvasLayer/ScrollContainer/Leaderboards");
		placementVbox = GetNode<VBoxContainer>("CanvasLayer/ScrollContainer/Leaderboards/placeContainer");
		nameVbox 	  = GetNode<VBoxContainer>("CanvasLayer/ScrollContainer/Leaderboards/nameContainer");
		scoreVbox 	  = GetNode<VBoxContainer>("CanvasLayer/ScrollContainer/Leaderboards/scoreContainer");

		scoreBtn = GetNode<Button>("CanvasLayer/btnScore");
		returnBtn = GetNode<Button>("CanvasLayer/btnReturn");
		playBtn = GetNode<Button>("CanvasLayer/btnPlay");
    }

    public void OnBtnPlayPressed(){
		var level = GetTree().ChangeSceneToFile(playLevel);
	}

	public void OnBtnExitPressed(){
		GetTree().Quit();
	}

	public void OnBtnLeaderboardsPressed(){
		playBtn.Visible = false;
		scoreBtn.Visible = false;
		
		returnBtn.Visible = true;
		scrollContainer.Visible = true;
		listScores();
	}

	public void OnBtnReturnPressed(){
		playBtn.Visible = true;
		scoreBtn.Visible = true;
		returnBtn.Visible = false;
		scrollContainer.Visible = false;
	}

	void listScores(){
		dbConnection = new SQLiteConnection($"Data Source= {dataPath}");
		dbConnection.Open();

		string query = "SELECT"+
  					   "(SELECT COUNT(DISTINCT Score) FROM OverallScores WHERE Score >= t.Score) AS placement,"+
  					   "Name, "+
  				       "Score "+
					   "FROM OverallScores t "+
					   "ORDER BY Score DESC";
		
		dbCmd = new SQLiteCommand(query, dbConnection);
		dbRead = dbCmd.ExecuteReader();

		List<Label> placementLabels = new List<Label>();
		List<Label> nameLabels = new List<Label>();
		List<Label> scoreLabels = new List<Label>();

		while (dbRead.Read())
		{
    		int placement = dbRead.GetInt32(dbRead.GetOrdinal("placement"));
    		string name = dbRead.GetString(dbRead.GetOrdinal("Name"));
    		int score = dbRead.GetInt32(dbRead.GetOrdinal("Score"));

    		Label placementLabel = new Label();
    		Label nameLabel = new Label();
    		Label scoreLabel = new Label();

    		placementLabel.Text = placement.ToString();
    		nameLabel.Text = name;
    		scoreLabel.Text = score.ToString("D12");

    		placementLabels.Add(placementLabel);
    		nameLabels.Add(nameLabel);
    		scoreLabels.Add(scoreLabel);
		}

		foreach (Label label in placementLabels)
		{
    		placementVbox.AddChild(label);
		}

		foreach (Label label in nameLabels)
		{
    		nameVbox.AddChild(label);
		}

		foreach (Label label in scoreLabels)
		{
    		scoreVbox.AddChild(label);
		}

		dbRead.Close();
		dbConnection.Close();
	}
}
