using Godot;
using System;

public partial class button : Button
{
	[Signal]
	public delegate void ClickEndEventHandler();
}
