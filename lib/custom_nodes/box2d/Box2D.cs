namespace leveling.lib.custom_nodes.box2d;

using addons.CustomScripts;
using Godot;

[Tool]
[CustomScript(nameof(Node2D))]
public partial class Box2D : Node2D {
    private Color _color = new(0.9f, 0.2f, 0.2f);
    private bool _isFilled;

    private float _size = 16;

    [Export]
    public float Size {
        get => _size;
        set {
            _size = value;
            QueueRedraw();
        }
    }

    [Export]
    public Color Color {
        get => _color;
        set {
            _color = value;
            QueueRedraw();
        }
    }

    [Export]
    public bool IsFilled {
        get => _isFilled;
        set {
            _isFilled = value;
            QueueRedraw();
        }
    }

    public override void _Draw() {
        DrawRect(new Rect2(new Vector2(-Size / 2, -Size / 2), new Vector2(Size, Size)), Color, IsFilled);
    }
}
