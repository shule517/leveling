namespace leveling.player;

using System.Linq;
using Godot;
using lib.extensions;
using monster;

[Tool]
public partial class Player : CharacterBody2D
{
    private int _hp = 10;

    private float _size = 16;
    private Color _color = new Color(0.9f, 0.2f, 0.2f);

    // ダメージ判定
    private bool _isDamage;
    private bool IsDamage
    {
        get => _isDamage;
        set { _isDamage = value; QueueRedraw(); }
    }

    public override void _Draw() => DrawRect(new Rect2(new Vector2(-_size / 2, -_size / 2), new Vector2(_size, _size)), _color, filled: IsDamage);

    [Export] public float Speed = 130f; // 移動速度

    public override void _PhysicsProcess(double delta)
    {
        if (Engine.IsEditorHint()) { return; }
        if (Input.IsActionJustPressed("button_y")) { AttackNormal(); }
        if (Input.IsActionJustPressed("button_b")) { AttackArea(); }

        // スティック入力を取得（左スティック）
        var horizontal = Input.GetJoyAxis(0, JoyAxis.LeftX);
        var vertical = Input.GetJoyAxis(0, JoyAxis.LeftY);

        // スティックの遊び（デッドゾーン）を考慮
        var deadZone = 0.2f;
        if (Mathf.Abs(horizontal) < deadZone) { horizontal = 0; }
        if (Mathf.Abs(vertical) < deadZone) { vertical = 0; }

        // GodotではY軸が下方向なので反転
        var inputVector = new Vector2(horizontal, vertical);
        Velocity = inputVector.Normalized() * Speed;

        MoveAndSlide();
    }

    // 通常攻撃
    private void AttackNormal()
    {
        GD.Print("AttackNormal!");
        var monster = GetTree().GetNodesInGroup("Monster").OfType<Poring>().OrderBy((monster) => Position.DistanceTo(monster.Position)).FirstOrDefault();
        monster?.Damage(1);
    }

    // 範囲攻撃
    private void AttackArea()
    {
        GD.Print("AttackArea!");
        var monsters = GetTree().GetNodesInGroup("Monster").OfType<Poring>();
        foreach (var monster in monsters)
        {
            monster.Damage(1);
        }
    }

    public async void Damage(int damage)
    {
        // 点滅
        IsDamage = true;
        await this.WaitSeconds(0.1f);
        IsDamage = false;

        _hp -= damage;
        GD.Print($"_hp: #{_hp}");
        if (_hp <= 0) { QueueFree(); }
    }
}
