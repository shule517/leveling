using Godot;
using System;

[Tool]
public partial class Player : CharacterBody2D
{
    private float _size = 16;
    private Color _color = new Color(0.9f, 0.2f, 0.2f);
    public override void _Draw() => DrawRect(new Rect2(new Vector2(-_size / 2, -_size / 2), new Vector2(_size, _size)), _color, filled: false);

    [Export] public float Speed = 130f; // 移動速度

    public override void _PhysicsProcess(double delta)
    {
        if (Engine.IsEditorHint())
        {
            return;
        }

        // スティック入力を取得（左スティック）
        float horizontal = Input.GetJoyAxis(0, JoyAxis.LeftX);
        float vertical = Input.GetJoyAxis(0, JoyAxis.LeftY);

        // スティックの遊び（デッドゾーン）を考慮
        const float deadzone = 0.2f;
        if (Mathf.Abs(horizontal) < deadzone) horizontal = 0;
        if (Mathf.Abs(vertical) < deadzone) vertical = 0;

        // GodotではY軸が下方向なので反転
        Vector2 inputVector = new Vector2(horizontal, vertical);
        Velocity = inputVector.Normalized() * Speed;

        MoveAndSlide();
    }
}
