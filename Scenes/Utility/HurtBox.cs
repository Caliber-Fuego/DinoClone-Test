using Godot;
using System;

public partial class HurtBox : Area2D
{
    [Export (PropertyHint.Enum, "Cooldown, Hitonce, DisableHitBox")] 
    public int HurtBoxType { get; set; } = 0;

    [Signal] public delegate void HurtEventHandler(int damage);
    
    CollisionShape2D collision;
    Timer disableTimer;

    public override void _Ready()
    {
        collision = GetNode<CollisionShape2D>("CollisionShape2D");
        disableTimer = GetNode<Timer>("DisableTimer");
    }

    public void OnAreaEntered(Area2D area){
        if (area.IsInGroup("attack")){
            if(!((int)area.Get("damage") == null)){
                switch (HurtBoxType){
                    case 0:
                        collision.CallDeferred("set", "disabled", true);
                        disableTimer.Start();
                    break;

                    case 1:
                    break;

                    case 2:
                        if (area.HasMethod("tempdisable")){
						        area.Call("tempdisable");
					    }   
                    break;
                }

                int damage = (int) area.Get("damage");

                EmitSignal(SignalName.Hurt, damage);
                if(area.HasMethod("enemyHit")){
                    area.Call("enemyHit");
                }
            }
        }
    }

    public void OnDisableTimerTimeout(){
        collision.CallDeferred("set", "disabled", false);
    }

}
