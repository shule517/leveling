namespace leveling.features.battle.floating_damage;

using Godot;
using lib.attributes;
using lib.extensions;

public partial class FloatingDamage : Node2D {
    private Vector2 _gravity = new(0, 1.0f);
    [Node] private Label _label = null!;
    private int _mass = 100;

    private float _moveX = -0.5f;
    private Vector2 _velocity = new(0, -90);

    public override void _Ready() {
        this.BindNodes();

        _moveX *= -1;
        _label.Text = GD.RandRange(80, 120).ToString();
        Scale = Vector2.One * 1.5f;

        var tween = CreateTween();
        tween.SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Quart);

        // 文字を徐々に透明にする
        tween.TweenProperty(this, "modulate", new Color(1.0f, 1.0f, 1.0f, 0), 2.0);
        tween.Parallel();

        // 文字を大きくする
        tween.TweenProperty(this, "scale", Vector2.One * 1.0f, 2.0);
        tween.TweenCallback(Callable.From(() => QueueFree()));
    }

    public override void _PhysicsProcess(double delta) {
        _velocity += _gravity * _mass * (float)delta;
        Position += _velocity * (float)delta;
        Position += new Vector2(_moveX, 0);
    }
}
