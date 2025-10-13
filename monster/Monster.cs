namespace leveling.monster;

using System.Threading.Tasks;
using Godot;
using lib.attributes;
using lib.custom_nodes.circle2d;
using lib.extensions;
using player;

[Tool]
public partial class Monster : CharacterBody2D
{
    [Export] public int Hp = 5;
    [Export] public bool IsActive; // アクティブ or ノンアクティブ
    [Export] public Color Color = new ("f27d73");

    private Player? _player;
    [Node] private Timer _attackTimer = null!;

    [Node] private Circle2D _circle2D = null!;

    // ダメージ判定
    private bool IsDamage
    {
        get => _circle2D.IsFilled;
        set => _circle2D.IsFilled = value;
    }

    // 追跡パラメータ
    private float _maxSpeed = 80f;     // 追跡速度
    private float _minDistance = 16f;  // Playerとの最小距離
    private float _slowDistance = 48f; // 減速開始距離

    // 描画
    private float _size = 16;

    public override void _Ready() => this.BindNodes();

    public override void _PhysicsProcess(double delta)
    {
        if (Engine.IsEditorHint()) { return; }
        if (_player == null) { return; }

        if (!_isChasing)
        {
            Velocity = Vector2.Zero;
            return;
        }

        var distance = _player.Position.DistanceTo(Position);
        if (distance < _minDistance)
        {
            Velocity = Vector2.Zero;
        }
        else
        {
            // 減速計算
            var speedFactor = 1f;
            if (distance < _slowDistance) { speedFactor = distance / _slowDistance; } // 近づくほど減速

            var direction = (_player.Position - Position).Normalized();
            Velocity = direction * _maxSpeed * speedFactor;
            MoveAndSlide();
        }
    }

    public async Task Damage(int damage)
    {
        if (!_isChasing) { _isChasing = true; }

        // 点滅
        IsDamage = true;

        // ノックバック
        if (_player != null)
        {
            var direction = (_player.Position - Position).Normalized() * -1;
            Position += direction * 16;
        }
        await this.WaitSeconds(0.1f);
        IsDamage = false;

        Hp -= damage;
        if (Hp <= 0) { QueueFree(); }
    }

    private bool _isChasing;
    private void _on_vision_area_2d_body_entered(Node2D body)
    {
        if (body is Player player)
        {
            _player = player;
            if (IsActive)
            {
                _isChasing = true;
            }
        }
    }

    private void _on_vision_area_2d_body_exited(Node2D body)
    {
        if (body is Player) { _isChasing = false; }
    }

    private async Task AttackPlayer()
    {
        var line = new Line2D { Width = 1, DefaultColor = Color, Points = new [] { Position, _player.Position },};
        GetParent().AddChild(line);
        await _player.Damage(1);
        line.QueueFree();
    }

    // TODO: 近づいてから攻撃した時に モンスターが攻撃をしてこない
    private void _on_attack_area_2d_body_entered(Node2D body)
    {
        if (body is Player player && _isChasing)
        {
            _player = player; // TODO: NodeなどでPlayer取得するべき？
            AttackPlayer();

            _attackTimer.Start(); // 2回目以降の攻撃タイマー
        }
    }

    private void _on_attack_area_2d_body_exited(Node2D body)
    {
        // タイマーを停止する
        if (body is Player) { _attackTimer.Stop(); }
    }

    private void _on_attack_timer_timeout() => AttackPlayer();
}
