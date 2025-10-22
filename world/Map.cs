using Godot;
using leveling.lib.attributes;
using leveling.lib.extensions;
using leveling.player;

public partial class Map : Node2D
{
    [Node] private Area2D _area2D = null!;
    // [Node] CollisionShape2D _collisionShape2D = null!;
    [Node] private CollisionShape2D _collisionShape2D = null!;

    public override void _Ready() {
        this.BindNodes();
        _area2D.BodyEntered += Area2DOnBodyEntered;

        // 最初から判定の中にいる場合
        foreach (var body in _area2D.GetOverlappingBodies())
        {
            Area2DOnBodyEntered(body);
        }
    }

    private void Area2DOnBodyEntered(Node2D body) {
        if (body is Player player) {
            var pos = GlobalPosition;
            var rect = (RectangleShape2D)_collisionShape2D.Shape;
            var size = rect.Size;

            var left   = (int)pos.X;
            var top    = (int)pos.Y;
            var right  = (int)(pos.X + size.X);
            var bottom = (int)(pos.Y + size.Y);

            player.SetCameraLimit(left, top, right, bottom);
        }
    }
}
