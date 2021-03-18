using Godot;
using System;

public class Mage : Node2D
{
    // Declare member variables here.

    private int speed = 120;
    private int cursorSpeed = 180;

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

        Vector2 cursorDir = new Vector2();

        cursorDir.x = Input.GetActionStrength("cursor_right") - Input.GetActionStrength("cursor_left");
        cursorDir.y = Input.GetActionStrength("cursor_down") - Input.GetActionStrength("cursor_up");

        if (dir.Length() > 1) {
            dir = dir.Normalized();
        }

        AnimatedSprite animation = (AnimatedSprite) GetNode("Body/AnimatedSprite");

        //Determine Character's rotation
        KinematicBody2D body = (KinematicBody2D) GetNode("Body");
        KinematicBody2D cursor = (KinematicBody2D) GetNode("Cursor");

        Vector2 diff = cursor.Position - body.Position;
        float angle = diff.Angle();

        bool flipLeft = (angle < -Math.PI/2 || angle > Math.PI/2);

        animation.FlipH = flipLeft;

        if (flipLeft) {
            angle -= (float) Math.PI;
        }

        animation.Rotation = angle;

        if (dir.Length() > 0) {
            animation.Play();
        }
        else {
            animation.Frame = 0;
            animation.FlipV = false;
            animation.Stop();
        }

        //Move character's body
        body.MoveAndSlide(dir * speed, new Vector2());

        //Move cursor
        cursor.MoveAndSlide(cursorDir * cursorSpeed, new Vector2());
    }
}
