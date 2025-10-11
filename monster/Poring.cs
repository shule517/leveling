using Godot;
using System;

[Tool]
public partial class Poring : CharacterBody2D
{
    private float _size = 16;
    private Color _color = new Color("f27d73");
    public override void _Draw() => DrawCircle(Vector2.Zero, _size / 2, _color);

    [Node] private Area2D _visionArea2D = null!;

    private void _on_vision_area_2d_body_entered(Node2D body)
    {
        if (body.IsInGroup("Player"))
        {
            var player = body as Player;
            Position = player.Position;
            GD.Print("Playerが視野に入りました！");
        }
    }
}
