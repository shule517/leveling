using Godot;
using leveling.lib;
using leveling.lib.attributes;
using leveling.lib.extensions;
using leveling.monster;
using leveling.player;

public partial class Map : Node2D
{
    [Node] private Area2D _area2D = null!;
    // [Node] CollisionShape2D _collisionShape2D = null!;
    [Node] private CollisionShape2D _collisionShape2D = null!;

    private static readonly Scene<Monster> _monsterScene =
        new("res://monster/monster.tscn");

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

            // モンスターを全部消す
            foreach (var node in GetTree().GetNodesInGroup("Monster")) node.QueueFree();

            // モンスターをランダムで配置する
            30.Times((i) => {
                var monster = _monsterScene.Instantiate();
                monster.Position = new Vector2(GD.RandRange(left, right), GD.RandRange(top, bottom));
                // TODO: 仮実装
                if (GD.RandRange(1, 2) == 1) {
                    monster.LineWidth = 1;
                    monster.Hp = 5;
                    monster.Name = "ポリン";
                    monster.WalkSpeed = (int)(Player.CellSize / (400 / 1000f));
                } else {
                    monster.LineWidth = 3;
                    monster.Hp = 8;
                    monster.Name = "ポポリン";
                    monster.WalkSpeed = (int)(Player.CellSize / (200 / 1000f));
                }
                GetParent().CallDeferred("add_child", monster);
            });
        }
    }
}
