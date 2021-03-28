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

    [Export]
    float aimConeMaxRadius = 120;

    [Export]
    float aimConeAngleDegrees = 2;

    bool useMouse = false;

    AnimatedSprite bodyAnimation;
    Sprite cursor;
    
    //Aim cone
    Area2D aimCone;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        bodyAnimation = GetNode<AnimatedSprite>("AnimatedSprite");
        cursor = GetNode<Sprite>("Cursor");
        aimCone = GetNode<Area2D>("AimCone");

        //Setup aim cone based off export variables (assumes character is facing right on startup)
        Vector2 playerDirection = new Vector2(1, 0);
        Vector2 diff = playerDirection.Tangent() * (float) Math.Tan(aimConeAngleDegrees * (Math.PI)/180) * aimConeMaxRadius;
        
        Vector2[] points = new Vector2[3];
        points[0] = Vector2.Zero;
        points[1] = playerDirection * aimConeMaxRadius - diff;
        points[2] = playerDirection * aimConeMaxRadius + diff;

        //Create collision shape, add to cone area2d object
        ConvexPolygonShape2D shape = new ConvexPolygonShape2D();
        shape.SetPointCloud(points);
        CollisionShape2D collisionShapeNode = new CollisionShape2D();
        collisionShapeNode.Shape = shape;
        aimCone.AddChild(collisionShapeNode);
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
        //Move character's body
        movePlayer();

        //Move cursor
        moveCursor();

        //Move aim cone
        moveAimCone();

    }

    private Vector2 getMovementDirection() {
        Vector2 movementDir = new Vector2();
        movementDir.x = Input.GetActionStrength("char_right") - Input.GetActionStrength("char_left");
        movementDir.y = Input.GetActionStrength("char_down") - Input.GetActionStrength("char_up");
        movementDir.Clamped(1);
        return movementDir;
    }

    private void movePlayer() {
        Vector2 movementDir = getMovementDirection();
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
    }

    //Returns the mouse position relative to the character's origin
    private Vector2 getRelativeMousePosition() {
        Vector2 mousePos = GetViewport().GetMousePosition();
        Vector2 characterPosRelativeToViewport = this.GetGlobalTransformWithCanvas().origin;
        mousePos -= characterPosRelativeToViewport;
        return mousePos;
    }

    //Returns a position vector for where the cursor should appear on screen
    // (NOTE this vector is not normalized, but is appropriately sized to position the aiming cursor)
    private Vector2 getCursorPosition() {
        if (useMouse) {
            Vector2 mousePos = GetViewport().GetMousePosition();
            Vector2 characterPosRelativeToViewport = this.GetGlobalTransformWithCanvas().origin;
            mousePos -= characterPosRelativeToViewport;
            return mousePos;
        }
        else {
            Vector2 aimingDir = new Vector2();
            aimingDir.x = Input.GetActionStrength("cursor_right") - Input.GetActionStrength("cursor_left");
            aimingDir.y = Input.GetActionStrength("cursor_down") - Input.GetActionStrength("cursor_up");
            aimingDir.Clamped(1);
            return aimingDir.Normalized() * (cursorMinDist + (aimingDir.Length() * (cursorMaxDist - cursorMinDist)));
        }
    }

    private void moveCursor() {
        Vector2 newCursorPos = getCursorPosition();

        //If we are currently aiming, move the cursor to that spot
        if (newCursorPos.Length() > 0.1) {
            cursor.Visible = true;
            cursor.Position = newCursorPos;
        }
        else {
            //Not aiming, hide cursor
            cursor.Visible = false;
        }
    }

    private void moveAimCone() {
        Vector2 coneDir = getCursorPosition();

        //If cursor direction doesn't exist, use movement direction
        if (coneDir.Length() < 0.1) {
            coneDir = getMovementDirection();
        }
        aimCone.Rotation = coneDir.Angle();
    }
}
