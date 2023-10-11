using Godot;
using System;

public partial class enemy_spawner : Node2D
{
	[Export] public Godot.Collections.Array<spawn_info> spawns { get; set; }
	[Export] int time = 0;

	[Signal]
	public delegate void ChangeTimeEventHandler (int time);

	Node2D player;
	Path2D spawnPath;
	PathFollow2D spawnLocation;

    public override void _Ready()
    {
        player = (Node2D) GetTree().GetFirstNodeInGroup("player");
		spawnPath = GetNode<Path2D>("Path2D");
		spawnLocation = GetNode<PathFollow2D>("Path2D/PathFollow2D");
    }

	public override void _PhysicsProcess(double delta)
    {

    }

	public void OnTimerTimeout(){
		time++;
		var enemy_spawns = spawns;
		foreach (var i in enemy_spawns){
			if (time >= i.timeStart && time <= i.timeEnd){
				if (i.spawnDelayCounter < i.enemySpawnDelay){
					i.spawnDelayCounter++;
				}else {
					i.spawnDelayCounter = 0;
					PackedScene new_enemy = (PackedScene) i.enemy;
					int counter = 0;
					while(counter < i.enemyNum){
						CharacterBody2D enemy_spawn = (CharacterBody2D)new_enemy.Instantiate();
						switch(i.enemySpawnType){
							case 1:
								spawnLocation.ProgressRatio = i.enemySpawnLocation;
							break;
							default:
								spawnLocation.ProgressRatio = GD.Randf();
							break;
						}
					enemy_spawn.Position = spawnLocation.Position;
					AddChild(enemy_spawn);
					counter++;
					}
				} 
			}
		}
	}
}
