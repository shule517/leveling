using Godot;
using System;

[Tool]
public partial class Poring : CharacterBody2D
{
    private float _size = 16;
    private Color _color = new Color("f27d73");
    public override void _Draw() => DrawCircle(Vector2.Zero, _size / 2, _color);

    [Node] private Area2D _visionArea2D = null!;
    private Player? _player;

    // 追跡パラメータ
    private float _maxSpeed = 80f;           // 追跡速度
    private float _minDistance = 16f;        // Playerとの最小距離
    private float _slowDistance = 48f;       // 減速開始距離

    public override void _PhysicsProcess(double delta)
    {
        if (Engine.IsEditorHint())
        {
            return;
        }

        if (_player == null)
        {
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

    private void _on_vision_area_2d_body_entered(Node body)
    {
        if (body.IsInGroup("Player"))
        {
            _player = body as Player;
        }
    }

    private void _on_vision_area_2d_body_exited(Node body)
    {
        if (body.IsInGroup("Player"))
        {
            _player = null;
        }
    }
}
