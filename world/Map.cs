using Godot;
using leveling.domains.extensions;
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

                var num = GD.RandRange(0, 3);
                if (num == 0) {
                    monster.LineWidth = 1;
                    monster.Hp = 5;
                    monster.Name = "ポリン";
                    monster.WalkSpeed = 400.ToWalkSpeed();
                    monster.AttackDelay = 1872; // 次に攻撃ができるまでの時間(ms)
                    // monster.AttackMotion = 672; // 攻撃中に動けない時間(ms) 1.8秒中の0.6秒が動けない
                    // monster.DamageMotion = 480; // ダメージを受けたあとの硬直時間(ms)
                    // monster.CastSensorChase = false; // 詠唱反応
                    // monster.Assist = false; // 同種モンスターが攻撃されてたら援護にくる
                    // monster.Looter = true; // アイテムを拾いにくる
                    monster.IsActive = false;
                } else if (num == 1) {
                    monster.LineWidth = 3;
                    monster.Hp = 8;
                    monster.Name = "ポポリン";
                    monster.WalkSpeed = 300.ToWalkSpeed();
                    monster.AttackDelay = 1672; // 次に攻撃ができるまでの時間(ms)
                    // monster.AttackMotion = 672;
                    // monster.DamageMotion = 480;
                    // monster.CastSensorChase = false;
                    // monster.Assist = false; // 同種モンスターが攻撃されてたら援護にくる
                    // monster.Looter = true; // アイテムを拾いにくる
                    monster.IsActive = false;
                } else if (num == 2) {
                    monster.LineWidth = 1;
                    monster.Hp = 12;
                    monster.Name = "マンドラゴラ";
                    monster.WalkSpeed = 0;
                    monster.AttackDelay = 1768;
                    // monster.AttackMotion = 768;
                    // monster.DamageMotion = 576;
                    // monster.CastSensorChase = false;
                    // monster.Assist = false; // 同種モンスターが攻撃されてたら援護にくる
                    // monster.Looter = false; // アイテムを拾いにくる
                    monster.AttackRange = 4;
                    monster.Color = new Color(1, 0, 0);
                    monster.IsActive = true;
                } else if (num == 3) {
                    monster.LineWidth = 1;
                    monster.Hp = 12;
                    monster.Name = "マーター";
                    monster.WalkSpeed = 150.ToWalkSpeed();
                    monster.AttackDelay = 432;
                    // monster.AttackMotion = 432;
                    // monster.DamageMotion = 360;
                    // monster.CastSensorChase = false;
                    // monster.Assist = false; // 同種モンスターが攻撃されてたら援護にくる
                    // monster.Looter = false; // アイテムを拾いにくる
                    monster.AttackRange = 1;
                    monster.Color = new Color(0, 0, 0);
                    monster.IsActive = true;
                }
                GetParent().CallDeferred("add_child", monster);
            });
        }
    }
}
