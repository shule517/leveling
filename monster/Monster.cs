namespace leveling.monster;

using System.Threading.Tasks;
using features.battle.floating_damage;
using Godot;
using lib;
using lib.attributes;
using lib.custom_nodes.circle2d;
using lib.extensions;
using player;

[Tool]
public partial class Monster : CharacterBody2D {
    private static readonly Scene<FloatingDamage> _floatingDamageScene =
        new("res://features/battle/floating_damage/floating_damage.tscn");

    [Node] private Timer _attackTimer = null!;
    [Node] private Timer _walkTimer = null!;
    [Node] private Circle2D _circle2D = null!;
    [Node] private Label _nameLabel = null!;

    private bool _isChasing;

    // 追跡パラメータ
    private float _maxSpeed = 80f; // 追跡速度
    private float _minDistance = 16f; // Playerとの最小距離

    private Player? _player;

    // 描画
    private float _size = 16;
    private float _slowDistance = 48f; // 減速開始距離

    [Export] public string Name = "ポリン";
    [Export] public Color Color = new("f27d73");
    [Export] public int Hp = 5;
    [Export] public bool IsActive; // アクティブ or ノンアクティブ

    // ダメージ判定
    private bool IsDamage {
        get => _circle2D.IsFilled;
        set => _circle2D.IsFilled = value;
    }

    public override void _Ready() {
        this.BindNodes();
        _nameLabel.Text = Name;
        _walkTimer.WaitTime = GD.RandRange(3, 10);
        _walkTimer.Timeout += WalkTimerOnTimeout;
        _walkTimer.Start();
    }

    private void WalkTimerOnTimeout() {
        GD.Print("WalkTimerOnTimeout");
        _walkTimer.WaitTime = GD.RandRange(3, 10);
        var toX = Position.X + (GD.RandRange(-5, 5) * Player.CellSize);
        var toY = Position.Y + (GD.RandRange(-5, 5) * Player.CellSize);
        CreateTween().TweenProperty(this, "position", new Vector2(toX, toY), 3);
    }

    public override void _PhysicsProcess(double delta) {
        if (Engine.IsEditorHint()) { return; }

        if (_player == null) { return; }

        if (!_isChasing) {
            Velocity = Vector2.Zero;
            return;
        }

        var distance = _player.Position.DistanceTo(Position);
        if (distance < _minDistance) {
            Velocity = Vector2.Zero;
        } else {
            // 減速計算
            var speedFactor = 1f;
            if (distance < _slowDistance) { speedFactor = distance / _slowDistance; } // 近づくほど減速

            var direction = (_player.Position - Position).Normalized();
            Velocity = direction * _maxSpeed * speedFactor;
            MoveAndSlide();
        }
    }

    public async Task Damage(int damage, float waitTime = 0.1f) {
        if (!_isChasing) { _isChasing = true; }

        // 点滅
        IsDamage = true;

        Input.StartJoyVibration(0, 0, 0.4f, 0.1f);

        // ダメージ表示
        var floatingDamage = _floatingDamageScene.Instantiate();
        floatingDamage.Position = Position;
        GetParent().AddChild(floatingDamage);

        // ノックバック
        if (_player != null) {
            var direction = (_player.Position - Position).Normalized() * -1;
            Position += direction * Player.CellSize;
        }

        await this.WaitSeconds(waitTime);
        IsDamage = false;

        Hp -= damage;
        if (Hp <= 0) { QueueFree(); }
    }

    private void _on_vision_area_2d_body_entered(Node2D body) {
        if (body is Player player) {
            _player = player;
            if (IsActive) {
                _isChasing = true;
            }
        }
    }

    private void _on_vision_area_2d_body_exited(Node2D body) {
        if (body is Player) { _isChasing = false; }
    }

    private async Task AttackPlayer() {
        var line = new Line2D { Width = 1, DefaultColor = Color, Points = new[] { Position, _player.Position } };
        GetParent().AddChild(line);
        await _player.Damage(1);
        line.QueueFree();
    }

    // TODO: 近づいてから攻撃した時に モンスターが攻撃をしてこない
    private void _on_attack_area_2d_body_entered(Node2D body) {
        if (body is Player player && _isChasing) {
            _player = player; // TODO: NodeなどでPlayer取得するべき？
            AttackPlayer();

            _attackTimer.Start(); // 2回目以降の攻撃タイマー
        }
    }

    private void _on_attack_area_2d_body_exited(Node2D body) {
        // タイマーを停止する
        if (body is Player) { _attackTimer.Stop(); }
    }

    private void _on_attack_timer_timeout() {
        AttackPlayer();
    }
}
