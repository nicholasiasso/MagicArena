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

    AnimatedSprite bodyAnimation;
    Sprite cursor;
    
    bool useMouse = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        bodyAnimation = GetNode<AnimatedSprite>("AnimatedSprite");
        cursor = GetNode<Sprite>("Cursor");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton eventMouseButton) {
            if (eventMouseButton.IsActionPressed("mouse_click")) {
                Input.SetMouseMode(Input.MouseMode.Hidden);
                useMouse = true;
            }
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(float delta)
    {
        Vector2 movementDir = new Vector2();
        movementDir.x = Input.GetActionStrength("char_right") - Input.GetActionStrength("char_left");
        movementDir.y = Input.GetActionStrength("char_down") - Input.GetActionStrength("char_up");
        movementDir.Clamped(1);

        //Move character's body
        this.MoveAndSlide(movementDir * speed, Vector2.Zero);
        if (movementDir.Length() > 0)
        {
            bodyAnimation.Play();
        }
        else
        {
            bodyAnimation.Frame = 0;
            bodyAnimation.Stop();
        }

        //Move cursor
        moveCursor();
        
    }

    private void moveCursor() {
        if (useMouse) {
            //Move cursor to mouse position
            Vector2 mousePos = GetViewport().GetMousePosition();
            Vector2 characterPosRelativeToViewport = this.GetGlobalTransformWithCanvas().origin;
            mousePos -= characterPosRelativeToViewport;
            GD.Print(mousePos);
            cursor.Visible = true;
            cursor.Position = mousePos;
        }
        else {
            Vector2 aimingDir = new Vector2();
            aimingDir.x = Input.GetActionStrength("cursor_right") - Input.GetActionStrength("cursor_left");
            aimingDir.y = Input.GetActionStrength("cursor_down") - Input.GetActionStrength("cursor_up");
            aimingDir.Clamped(1);
            if (aimingDir.Length() > 0.1)
            {
                setCursorPosition(aimingDir);
            }
            else
            {
                cursor.Visible = false;
            }
        }
    }

    private void setCursorPosition(Vector2 dir) {
        cursor.Visible = true;
        cursor.Position = dir.Normalized() * (cursorMinDist + (dir.Length() * (cursorMaxDist - cursorMinDist)));
    }
}
