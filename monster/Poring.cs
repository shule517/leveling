namespace leveling.monster;

using System.Threading.Tasks;
using Godot;
using lib.attributes;
using lib.extensions;
using player;

[Tool]
public partial class Poring : CharacterBody2D
{
    private int _hp = 5;

    private float _size = 16;
    private Color _color = new Color("f27d73");

    private Player? _player;
    [Node] private Timer _attackTimer = null!;

    // ダメージ判定
    private bool _isDamage;
    private bool IsDamage
    {
        get => _isDamage;
        set { _isDamage = value; QueueRedraw(); }
    }

    // 追跡パラメータ
    private float _maxSpeed = 80f;     // 追跡速度
    private float _minDistance = 16f;  // Playerとの最小距離
    private float _slowDistance = 48f; // 減速開始距離

    public override void _Draw() => DrawCircle(Vector2.Zero, _size / 2, _color, filled: IsDamage);
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
        // 点滅
        IsDamage = true;
        await this.WaitSeconds(0.1f);
        IsDamage = false;

        _hp -= damage;
        if (_hp <= 0) { QueueFree(); }
    }

    private bool _isChasing;
    private void _on_vision_area_2d_body_entered(Node2D body)
    {
        if (body is Player player)
        {
            _player = player;
            _isChasing = true;
        }
    }

    private void _on_vision_area_2d_body_exited(Node2D body)
    {
        if (body is Player) { _isChasing = false; }
    }

    private void _on_attack_area_2d_body_entered(Node2D body)
    {
        if (body is Player player)
        {
            _player = player; // TODO: NodeなどでPlayer取得するべき？
            player.Damage(1);
            _attackTimer.Start(); // 2回目以降の攻撃タイマー
        }
    }

    private void _on_attack_area_2d_body_exited(Node2D body)
    {
        // タイマーを停止する
        if (body is Player) { _attackTimer.Stop(); }
    }

    private void _on_attack_timer_timeout() => _player?.Damage(1);
}
