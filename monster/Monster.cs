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
    private static readonly Scene<FloatingDamage> _floatingDamageScene = new("res://features/battle/floating_damage/floating_damage.tscn");

    // パラメータ
    [Export] public string Name = "ポリン";
    [Export] public int LineWidth = 1;
    [Export] public int Hp = 5;
    [Export] public bool IsActive; // アクティブ or ノンアクティブ
    [Export] public Color Color = new("f27d73");
    [Export] public int WalkSpeed = 100; // 1秒間に100px動く
    [Export] public int AttackDelay = 1800;
    [Export] public int AttackRange = 1;
    [Export] public int ChaseRange = 12;

    // component
    [Node] private Timer _attackTimer = null!;
    [Node] private Timer _walkTimer = null!;
    [Node] private Circle2D _circle2D = null!;
    [Node] private Label _nameLabel = null!;
    [Node] private Area2D _viewArea2D = null!;
    [Node("ViewArea2D/CollisionShape2D")] private CollisionShape2D _viewCollisionShape2D = null!;
    [Node("AttackArea2D/CollisionShape2D")] private CollisionShape2D _attackCollisionShape2D = null!;

    private bool _isChasing; // 追跡中
    private float _minDistance = 16f; // Playerとの最小距離

    private Player? _player;

    // 描画
    private float _size = 16;
    private float _slowDistance = 48f; // 減速開始距離

    // ダメージ判定
    private bool IsDamage {
        get => _circle2D.IsFilled;
        set {
            _circle2D.IsFilled = value;
            _walkTween?.Stop();
        }
    }

    public override void _Ready() {
        this.BindNodes();

        // モンスターの本体の描画設定
        _circle2D.Color = Color;
        _circle2D.LineWidth = LineWidth;
        _nameLabel.Text = Name;

        // 攻撃範囲
        ((CircleShape2D)_attackCollisionShape2D.Shape).Radius = AttackRange * Player.CellSize;
        // 追跡範囲
        ((CircleShape2D)_viewCollisionShape2D.Shape).Radius = ChaseRange * Player.CellSize;

        // 攻撃
        _attackTimer.WaitTime = AttackDelay / 1000f;

        // モンスターの感知
        _viewArea2D.BodyEntered += _on_view_area_2d_body_entered;
        _viewArea2D.BodyExited += _on_view_area_2d_body_exited;

        // 歩くタイマー
        _walkTimer.WaitTime = GD.RandRange(0.1, 10);
        _walkTimer.Timeout += WalkTimerOnTimeout;
        _walkTimer.Start();
    }

    // 歩くモーション
    private Tween? _walkTween;
    private void WalkTimerOnTimeout() {
        _walkTimer.WaitTime = GD.RandRange(3, 10);

        if (!_isChasing) {
            _walkTween?.Stop();
            _walkTween = CreateTween();
            var toX = Position.X + (GD.RandRange(-5, 5) * Player.CellSize);
            var toY = Position.Y + (GD.RandRange(-5, 5) * Player.CellSize);
            var toPosition = new Vector2(toX, toY);

            var duration = Position.DistanceTo(toPosition) / WalkSpeed;
            _walkTween.TweenProperty(this, "position", toPosition, duration);
        }
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
            Velocity = direction * WalkSpeed * speedFactor;
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

    private void _on_view_area_2d_body_entered(Node2D body) {
        if (body is Player player) {
            _player = player;
            if (IsActive) {
                _isChasing = true;
            }
        }
    }

    private void _on_view_area_2d_body_exited(Node2D body) {
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
