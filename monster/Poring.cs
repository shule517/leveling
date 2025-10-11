using Godot;
using System;

[Tool]
public partial class Poring : CharacterBody2D
{
    private float _size = 16;
    private Color _color = new Color("f27d73");
    public override void _Draw() => DrawCircle(Vector2.Zero, _size / 2, _color);

    [Node] private Area2D _visionArea2D = null!;
}
