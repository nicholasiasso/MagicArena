using Godot;
using System;

public class Mage : Node2D
{
    // Declare member variables here.

    private int speed = 120;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(float delta)
    {
        Vector2 dir = new Vector2();

        dir.x = Input.GetActionStrength("char_right") - Input.GetActionStrength("char_left");
        dir.y = Input.GetActionStrength("char_down") - Input.GetActionStrength("char_up");

        if (dir.Length() > 1) {
            dir = dir.Normalized();
        }

        Vector2 vel = dir * speed;

        AnimatedSprite animation = (AnimatedSprite) GetNode("Area2D/AnimatedSprite");

        if (vel.Length() > 0) {
            animation.Play();
        }
        else {
            animation.Frame = 0;
            animation.FlipV = false;
            animation.Stop();
        }

        this.Position = this.Position + (vel * delta);

        this.Rotation = dir.Angle();
        
    }
}
