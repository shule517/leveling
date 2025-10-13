namespace leveling.lib.custom_nodes.circle2d;

using Godot;
using addons.CustomScripts;

[Tool]
[CustomScript(nameof(Node2D))]
public partial class Circle2D : Node2D
{
    public override void _Draw() => DrawCircle(Vector2.Zero, Size / 2, Color, filled: IsFilled);

    private float _size = 16;
    private Color _color = new Color(0.9f, 0.2f, 0.2f);
    private bool _isFilled;

    [Export] public float Size
    {
        get => _size;
        set { _size = value; QueueRedraw(); }
    }

    [Export] public Color Color
    {
        get => _color;
        set { _color = value; QueueRedraw(); }
    }

    [Export] public bool IsFilled
    {
        get => _isFilled;
        set { _isFilled = value; QueueRedraw(); }
    }
}
