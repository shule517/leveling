namespace leveling.player;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    [Export] public Vector2 RoomSize = new(512 * 2, 352 * 2); // 1部屋のサイズ
    [Export] public float Speed = 130f; // 移動速度

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
        UpdateCameraLimits();
    }

    private void UpdateCameraLimits() {
        _currentRoom = new Vector2((int)(Position.X / (int)RoomSize.X), (int)(Position.Y / (int)RoomSize.Y));
        var left = _currentRoom.X * RoomSize.X;
        var top = _currentRoom.Y * RoomSize.Y;

        _camera2d.LimitLeft = (int)left;
        _camera2d.LimitRight = (int)(left + RoomSize.X);
        _camera2d.LimitTop = (int)top;
        _camera2d.LimitBottom = (int)(top + RoomSize.Y);
    }

    public override void _PhysicsProcess(double delta) {
        if (Engine.IsEditorHint()) { return; }

        // スタン中なので移動できない
        if (_isStunned) { return; }

        if (!CanMove) { return; }

        // 左スティックで移動
        var input = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        Velocity = input * Speed;
        MoveAndSlide();

        UpdateCameraLimits();
    }

    public async Task Damage(int damage) {
        Input.StartJoyVibration(0, 0.4f, 0, 0.1f);

        // 点滅
        IsDamage = true;
        _isStunned = true;
        await this.WaitSeconds(0.1f);
        IsDamage = false;
        _isStunned = false;

        Hp -= damage;
        if (Hp <= 0) {
            Position = _startPosition;
            Hp = 30;
        }
    }
}
