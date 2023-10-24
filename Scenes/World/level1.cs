using Godot;
using System;

public partial class level1 : Node2D
{
	AudioStreamPlayer level1BGM;

    public override void _Ready()
    {
    }

	public void OnBGMPlayFinished(){
		level1BGM = GetNode<AudioStreamPlayer>("bgm");
		level1BGM.Play();
	}

}
