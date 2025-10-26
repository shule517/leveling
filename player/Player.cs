namespace leveling.player;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using domains.extensions;
using Godot;
using lib.attributes;
using lib.custom_nodes.box2d;
using lib.extensions;
using monster;

public partial class Player : CharacterBody2D {
    public const float CellSize = 16;

    [Node] private Box2D _box2d = null!;
    [Node] private Camera2D _camera2d = null!;
    private Vector2 _currentRoom = Vector2.Zero;
    private int _hp = 20;

    private bool _isStunned; // モンスターから攻撃をくらって、スタン状態

    private int _sp = 20;

    private Vector2 _startPosition;

    [Export] public Color Color = new(0.9f, 0.2f, 0.2f);
    [Export] public int WalkSpeed; // 歩く速度

    [Export]
    public int Hp {
        get => _hp;
        set {
            _hp = value;
            GD.Print($"HP: {value}");
        }
    }

    [Export]
    public int Sp {
        get => _sp;
        set {
            _sp = value;
            GD.Print($"SP: {value}");
        }
    }

    // ダメージ判定
    private bool IsDamage {
        get => _box2d.IsFilled;
        set => _box2d.IsFilled = value;
    }

    private List<Monster> Monsters => GetTree().GetNodesInGroup("Monster").Cast<Monster>().ToList();
    public bool CanMove { get; set; } = true;

    public IEnumerable<Monster> AttackAreaMonsters(int rangeCell) {
        return Monsters.Where(monster => Position.DistanceTo(monster.Position) < CellSize * rangeCell);
    }

    public Monster? AttackMonster(int rangeCell) {
        return Monsters
            .Where(monster => Position.DistanceTo(monster.Position) < CellSize * rangeCell)
            .OrderBy(monster => Position.DistanceTo(monster.Position))
            .FirstOrDefault();
    }

    public override void _Ready() {
        _startPosition = Position;
        this.BindNodes();
        WalkSpeed = 150.ToWalkSpeed();
    }

    public override void _PhysicsProcess(double delta) {
        if (Engine.IsEditorHint()) { return; }

        // スタン中なので移動できない
        if (_isStunned) { return; }

        if (!CanMove) { return; }

        // 左スティックで移動
        var input = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        Velocity = input * WalkSpeed;
        MoveAndSlide();

        float speed = 1.0f;
        float t = (float)(1 - Math.Pow(0.001, delta * speed));
        _camera2d.LimitLeft = (int)Mathf.Lerp(_camera2d.LimitLeft, _targetLeft, t);
        _camera2d.LimitTop = (int)Mathf.Lerp(_camera2d.LimitTop, _targetTop, t);
        _camera2d.LimitRight = (int)Mathf.Lerp(_camera2d.LimitRight, _targetRight, t);
        _camera2d.LimitBottom = (int)Mathf.Lerp(_camera2d.LimitBottom, _targetBottom, t);
    }

    public async Task Damage(int damage) {
        Input.StartJoyVibration(0, 0.4f, 0, 0.1f);

        // 点滅
        IsDamage = true;
        _isStunned = true;
        await this.WaitSeconds(0.19f); // ダメージ硬直
        IsDamage = false;
        _isStunned = false;

        Hp -= damage;
        if (Hp <= 0) {
            Position = _startPosition;
            Hp = 30;
        }
    }

    private int _targetLeft = 0;
    private int _targetTop = 0;
    private int _targetRight = 0;
    private int _targetBottom = 0;
    public void SetCameraLimit(int left, int top, int right, int bottom) {
        GD.Print($"left: {left}, top: {top}, right: {right}, bottom: {bottom}");

        _targetLeft = left;
        _targetTop = top;
        _targetRight = right;
        _targetBottom = bottom;

        // var tween = CreateTween();
        // tween.SetTrans(Tween.TransitionType.Sine)
        //     .SetEase(Tween.EaseType.InOut);
        //
        // // limit値も同時に補間
        // tween.Parallel().TweenProperty(_camera2d, "limit_left", left, 1.0f);
        // tween.Parallel().TweenProperty(_camera2d, "limit_top", top, 1.0f);
        // tween.Parallel().TweenProperty(_camera2d, "limit_right", right, 1.0f);
        // tween.Parallel().TweenProperty(_camera2d, "limit_bottom", bottom, 1.0f);
    }
}
