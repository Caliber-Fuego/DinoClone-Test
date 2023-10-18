using Godot;
using System;

public partial class character_select : Control
{
	public string characterName = "Test";
	string playLevel = "res://Scenes/World/main.tscn";


	//GUI
	VideoStreamPlayer selectBG;
	Label nameLabel;
	TextureButton startBtn, eclipseBtn, sayakaBtn;
	LineEdit userInput;
	Player player;
	Global global;

	public override void _Ready()
	{
		global = GetNode<Global>("/root/Global");

		startBtn = GetNode<TextureButton>("StartBtn");
		eclipseBtn = GetNode<TextureButton>("EclipseBtn");
		sayakaBtn = GetNode<TextureButton>("SayakaBtn");
		userInput = GetNode<LineEdit>("LineEdit");
		nameLabel = GetNode<Label>("Label");
	}

	public void OnVideoStreamPlayFinished(){
		selectBG = GetNode<VideoStreamPlayer>("VideoStreamPlayer");
		selectBG.Play();
	}

	public void OnEclipseBtnPressed(){
		startBtn.Visible = true;
		userInput.Visible = true;
		nameLabel.Visible = true;
		userInput.Text = "Eclipse";
		global.spritePath = "EclipseSprite";

		eclipseBtn.Visible = false;
		sayakaBtn.Visible = false;
	}

	public void OnSayakaBtnPressed(){
		startBtn.Visible = true;
		userInput.Visible = true;
		nameLabel.Visible = true;
		userInput.Text = "Sayaka";
		global.spritePath = "SayakaSprite";

		eclipseBtn.Visible = false;
		sayakaBtn.Visible = false;
	}

	public void OnStartBtnPressed(){
		global.userName = userInput.Text;
		var level = GetTree().ChangeSceneToFile(playLevel);

	}
}
