namespace leveling.monster;
using Godot;
using lib.extensions;
using player;

[Tool]
public partial class Poring : CharacterBody2D
{
    private int _hp = 4;

    private float _size = 16;
    private Color _color = new Color("f27d73");

    private Player? _player;

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

    public override void _PhysicsProcess(double delta)
    {
        if (Engine.IsEditorHint()) { return; }
        if (_player == null) { return; }

        var distance = _player.Position.DistanceTo(Position);
        if (distance < _minDistance)
        {
            Velocity = Vector2.Zero;
        }
        else
        {
            // 減速計算
            var speedFactor = 1f;
            if (distance < _slowDistance)
            {
                speedFactor = distance / _slowDistance; // 近づくほど減速
            }

            var toPlayer = _player.Position - Position;
            var direction = toPlayer.Normalized();
            Velocity = direction * _maxSpeed * speedFactor;
            MoveAndSlide();
        }
    }

    public async void Damage(int damage)
    {
        // 点滅
        IsDamage = true;
        await this.WaitSeconds(0.1f);
        IsDamage = false;

        _hp -= damage;
        if (_hp <= 0) { QueueFree(); }
    }

    private void _on_vision_area_2d_body_entered(Node body)
    {
        if (body is Player player) { _player = player; }
    }

    private void _on_vision_area_2d_body_exited(Node body)
    {
        if (body is Player) { _player = null; }
    }

    private void _on_atack_area_2d_body_entered(Node body)
    {
        if (body is Player player) { player.Damage(1); }
    }

    private void _on_atack_area_2d_body_exited(Node body)
    {
        // if (body is Player) { _player = null; }
    }
}
