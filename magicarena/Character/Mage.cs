using Godot;
using System;

public class Mage : KinematicBody2D
{
    [Export]
    float cursorMinDist = 30;

    [Export]
    float cursorMaxDist = 40;

    [Export]
    float speed = 150;




    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        AnimatedSprite animation = (AnimatedSprite) GetNode("AnimatedSprite");
        Sprite cursor = (Sprite) GetNode("Cursor");

        Vector2 movementDir = new Vector2();
        movementDir.x = Input.GetActionStrength("char_right") - Input.GetActionStrength("char_left");
        movementDir.y = Input.GetActionStrength("char_down") - Input.GetActionStrength("char_up");
        movementDir.Clamped(1);

        Vector2 aimingDir = new Vector2();
        aimingDir.x = Input.GetActionStrength("cursor_right") - Input.GetActionStrength("cursor_left");
        aimingDir.y = Input.GetActionStrength("cursor_down") - Input.GetActionStrength("cursor_up");
        aimingDir.Clamped(1);

        //Move character's body
        this.MoveAndSlide(movementDir * speed, Vector2.Zero);
        if (movementDir.Length() > 0)
        {
            animation.Play();
        }
        else
        {
            animation.Frame = 0;
            animation.Stop();
        }

        //Move cursor
        if (aimingDir.Length() > 0.1)
        {
            cursor.Visible = true;
            cursor.Position = aimingDir.Normalized() * (cursorMinDist + (aimingDir.Length() * (cursorMaxDist - cursorMinDist)));
        }
        else
        {
            cursor.Visible = false;
        }
    }
}
