using Godot;
using System;

[GlobalClass]
public partial class spawn_info : Resource
{
	[Export] public int timeStart;
	[Export] public int timeEnd;
	[Export] public Resource enemy;
	[Export] public int enemyNum;
	[Export] public int enemySpawnDelay;
	[Export] public int enemySpawnType = 0;
	[Export(PropertyHint.Range, "0,1,")] public float enemySpawnLocation;

	public int spawnDelayCounter = 0;

}
