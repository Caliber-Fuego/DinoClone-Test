using Godot;
using System;
using System.Data.SQLite;


public partial class Leaderboards : GridContainer
{
	//Database
	public string dataPath {get;} = "C:/Users/Asus/Documents/School Documents/Object Oriented Programming/Tests/DinoClone Test/Database/data.db";
	SQLiteConnection dbConnection;
	SQLiteDataReader dbRead;
	SQLiteCommand dbCmd;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//Database Related
		dbConnection = new SQLiteConnection();
		listLeaderboards();
	}

	public void listLeaderboards(){
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
        	scoreLabels[row].Text = score.ToString();

        	row++;
    		}
		}
		
		var vbox = new VBoxContainer();
		for (int i = 0; i < numberOfRows; i++)
		{
    		vbox.AddChild(placementLabels[i]);
    		vbox.AddChild(nameLabels[i]);
    		vbox.AddChild(scoreLabels[i]);
		}

		dbRead.Close();
		dbConnection.Close();
	}
}
