namespace leveling.player;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using lib.attributes;
using lib.custom_nodes.box2d;
using lib.extensions;
using monster;

public partial class Player : CharacterBody2D
{
    private int _hp = 10;

    private float _size = 16;
    private Color _color = new Color(0.9f, 0.2f, 0.2f);

    private bool _isStunned; // モンスターから攻撃をくらって、スタン状態

    [Node] private Box2D _box2D = null!;

    // ダメージ判定
    private bool IsDamage
    {
        get => _box2D.IsFilled;
        set => _box2D.IsFilled = value;
    }

    public override void _Ready()
    {
        Position = _startPosition;
        this.BindNodes();
        UpdateCameraLimits();
    }

    // public override void _Draw() => DrawRect(new Rect2(new Vector2(-_size / 2, -_size / 2), new Vector2(_size, _size)), _color, filled: IsDamage);

    [Export] public float Speed = 130f; // 移動速度

    private bool _canAttackNormal = true;
    private bool _canAttackArea = true;

    [Export] public Vector2 RoomSize = new Vector2(512*2, 352*2); // 1部屋のサイズ
    private Vector2 _currentRoom = Vector2.Zero;
    [Node] private Camera2D _camera2d = null!;

    private void UpdateCameraLimits()
    {
        _currentRoom = new Vector2((int)(Position.X / (int)RoomSize.X), (int)(Position.Y / (int)RoomSize.Y));
        var left = _currentRoom.X * RoomSize.X;
        var top = _currentRoom.Y * RoomSize.Y;

        _camera2d.LimitLeft = (int)left;
        _camera2d.LimitRight = (int)(left + RoomSize.X);
        _camera2d.LimitTop = (int)top;
        _camera2d.LimitBottom = (int)(top + RoomSize.Y);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Engine.IsEditorHint()) { return; }
        if (_canAttackNormal && Input.IsActionJustPressed("button_y")) { AttackNormal(); }
        if (_canAttackArea && Input.IsActionJustPressed("button_b")) { AttackArea(); }

        // スタン中なので移動できない
        if (_isStunned) { return; }

        // 左スティックで移動
        var input = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        Velocity = input * Speed;
        MoveAndSlide();

        CheckRoomTransition();
    }

    private void CheckRoomTransition()
    {
        // TODO: 仮実装
        UpdateCameraLimits();
    }

    // 通常攻撃
    private async Task AttackNormal()
    {
        var monster = _attackMonsters.OrderBy((monster) => Position.DistanceTo(monster.Position)).FirstOrDefault();
        if (monster == null) { return; }

        _canAttackNormal = false;
        monster?.Damage(1);

        var line = new Line2D { Width = 1, DefaultColor = _color, Points = new [] { Position, monster.Position },};
        GetParent().AddChild(line);
        await this.WaitSeconds(0.1f);
        line.QueueFree();

        await this.WaitSeconds(1.0f);
        _canAttackNormal = true;
    }

    // 範囲攻撃
    private async Task AttackArea()
    {
        _canAttackArea = false;
        foreach (var monster in _attackMonsters)
        {
            monster.Damage(1);
        }

        await this.WaitSeconds(2.0f);
        _canAttackArea = true;
    }

    public async Task Damage(int damage)
    {
        // 点滅
        IsDamage = true;
        _isStunned = true;
        await this.WaitSeconds(0.1f);
        IsDamage = false;
        _isStunned = false;

        _hp -= damage;
        GD.Print($"_hp: #{_hp}");
        if (_hp <= 0)
        {
            Position = _startPosition;
            _hp = 30;
        }
    }

    private Vector2 _startPosition = new(100, 100);

    private readonly List<Monster> _attackMonsters = new();
    private void _on_attack_area_2d_body_entered(Node2D body)
    {
        if (body is Monster monster) { _attackMonsters.Add(monster); }
    }

    private void _on_attack_area_2d_body_exited(Node2D body)
    {
        if (body is Monster monster) { _attackMonsters.Remove(monster); }
    }
}
