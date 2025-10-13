namespace leveling.player;

using System.Threading.Tasks;
using Godot;
using lib.attributes;
using lib.custom_nodes.box2d;
using lib.extensions;

public partial class Player : CharacterBody2D
{
    [Export] public int Hp = 10;
    [Export] public Color Color = new(0.9f, 0.2f, 0.2f);
    [Export] public float Speed = 130f; // 移動速度
    [Export] public Vector2 RoomSize = new Vector2(512*2, 352*2); // 1部屋のサイズ

    [Node] private Box2D _box2d = null!;
    [Node] private Camera2D _camera2d = null!;

    private Vector2 _startPosition;
    private Vector2 _currentRoom = Vector2.Zero;

    private bool _isStunned; // モンスターから攻撃をくらって、スタン状態

    // ダメージ判定
    private bool IsDamage
    {
        get => _box2d.IsFilled;
        set => _box2d.IsFilled = value;
    }

    public override void _Ready()
    {
        _startPosition = Position;
        this.BindNodes();
        UpdateCameraLimits();
    }

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

        // スタン中なので移動できない
        if (_isStunned) { return; }

        // 左スティックで移動
        var input = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        Velocity = input * Speed;
        MoveAndSlide();

        UpdateCameraLimits();
    }

    public async Task Damage(int damage)
    {
        Input.StartJoyVibration(0, 0.4f, 0, 0.1f);

        // 点滅
        IsDamage = true;
        _isStunned = true;
        await this.WaitSeconds(0.1f);
        IsDamage = false;
        _isStunned = false;

        Hp -= damage;
        GD.Print($"Hp: #{Hp}");
        if (Hp <= 0)
        {
            Position = _startPosition;
            Hp = 30;
        }
    }
}
