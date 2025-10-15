namespace leveling.player.job_state;
using Godot;

public partial class Knight : JobState
{
    public override void OnReady()
    {
        // GD.Print("Knight.OnReady");
    }

    public override void OnProcess()
    {
        // GD.Print("Knight.OnProcess");
    }

    public override void OnPhysicsProcess()
    {
        // GD.Print("Knight.OnPhysicsProcess");
    }

    // private Player Player => (Player)Owner;
    //
    // private bool _canAttackNormal = true;
    // private bool _canAttackArea = true;
    //
    // public override void _PhysicsProcess(double delta)
    // {
    //     GD.Print("Knight");
    //     if (_canAttackNormal && Input.IsActionJustPressed("button_y")) { AttackNormal(); }
    //     if (_canAttackArea && Input.IsActionJustPressed("button_b")) { AttackArea(); }
    // }
    //
    // // 通常攻撃
    // private async Task AttackNormal()
    // {
    //     var monster = _attackMonsters.OrderBy((monster) => Player.Position.DistanceTo(monster.Position)).FirstOrDefault();
    //     if (monster == null) { return; }
    //
    //     _canAttackNormal = false;
    //     monster.Damage(1);
    //
    //     var points = new[] { Player.ToLocal(Player.Position), Player.ToLocal(monster.Position) };
    //     var line = new Line2D { Width = 1, DefaultColor = Player.Color, Points = points };
    //     Player.AddChild(line);
    //     await this.WaitSeconds(0.1f);
    //     line.QueueFree();
    //
    //     await this.WaitSeconds(1.0f);
    //     _canAttackNormal = true;
    // }
    //
    // // 範囲攻撃
    // private async Task AttackArea()
    // {
    //     _canAttackArea = false;
    //     var circle = new Circle2D { Size = 200, Color = new Color(1, 1, 1, 0.3f), IsFilled = true };
    //     Player.AddChild(circle);
    //     foreach (var monster in _attackMonsters)
    //     {
    //         monster.Damage(1);
    //     }
    //     await this.WaitSeconds(0.1f);
    //     Player.RemoveChild(circle);
    //
    //     await this.WaitSeconds(2.0f);
    //     _canAttackArea = true;
    // }
    //
    // private readonly List<Monster> _attackMonsters = new();
    // private void _on_attack_area_2d_body_entered(Node2D body)
    // {
    //     if (body is Monster monster) { _attackMonsters.Add(monster); }
    // }
    //
    // private void _on_attack_area_2d_body_exited(Node2D body)
    // {
    //     if (body is Monster monster) { _attackMonsters.Remove(monster); }
    // }
}
