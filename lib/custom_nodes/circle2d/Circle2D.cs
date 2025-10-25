namespace leveling.lib.custom_nodes.circle2d;

using addons.CustomScripts;
using Godot;

[Tool]
[CustomScript(nameof(Node2D))]
public partial class Circle2D : Node2D {
    private Color _color = new(0.9f, 0.2f, 0.2f);
    private bool _isFilled;
    private float _size = 16;
    private int _lineWidth = 1;

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

    [Export]
    public int LineWidth {
        get => _lineWidth;
        set {
            _lineWidth = value;
            QueueRedraw();
        }
    }

    public override void _Draw() {
        if (IsFilled) {
            // filledの時にlineWidthを設定できない
            DrawCircle(Vector2.Zero, Size / 2, Color, IsFilled);
        } else {
            DrawCircle(Vector2.Zero, Size / 2, Color, IsFilled, LineWidth);
        }
    }
}
