using Godot;
using System;

[Tool]
public partial class Field : Node2D
{
    public override void _Draw()
    {
        var gridSize = 64;
        var color = new Color(0.2f, 0.2f, 0.2f);
        for (int x = 0; x < 64; x++)
        {
            DrawLine(new Vector2(x * gridSize, 0), new Vector2(x * gridSize, 64 * 64), color, 2);
            for (int y = 0; y < 64; y++)
            {
                DrawLine(new Vector2(0, y * gridSize), new Vector2(64 * 64, y * gridSize), color, 2);
            }
        }
    }
}
