using Godot;
using System;

public partial class Player : Node2D
{
    [Export] public float Size = 16;
    [Export] public Color Color = new Color(0.9f, 0.2f, 0.2f);

    public override void _Draw()
    {
        DrawRect(new Rect2(Vector2.Zero, new Vector2(Size, Size)), Color);
    }
}
