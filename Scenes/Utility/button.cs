using Godot;
using System;

public partial class button : Button
{
	[Signal]
	public delegate void ClickEndEventHandler();

	AudioStreamPlayer sndHover, sndClick;

    public override void _Ready()
    {
        sndHover = GetNode<AudioStreamPlayer>("sndHover");
        sndClick = GetNode<AudioStreamPlayer>("sndClick");
    }

    public void OnMouseEntered(){
		 sndHover.Play();
	}

	public void OnPressed(){
		sndClick.Play();
	}

	public void OnSndClickFinished(){
		EmitSignal(SignalName.ClickEnd);
	}
}
