using Godot;
using System;

public partial class title_screen : Control
{
	string playLevel = "res://Scenes/World/main.tscn";

	public void OnBtnPlayPressed(){
		var level = GetTree().ChangeSceneToFile(playLevel);
	}

	public void OnBtnExitPressed(){
		GetTree().Quit();
	}
}
