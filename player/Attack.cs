using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using leveling.lib.extensions;
using leveling.monster;
using leveling.player;

public partial class Attack : Node
{
    private Player Player => (Player)Owner;

    private bool _canAttackNormal = true;
    private bool _canAttackArea = true;

    public override void _PhysicsProcess(double delta)
    {
        if (_canAttackNormal && Input.IsActionJustPressed("button_y")) { AttackNormal(); }
        if (_canAttackArea && Input.IsActionJustPressed("button_b")) { AttackArea(); }
    }

    // 通常攻撃
    private async Task AttackNormal()
    {
        var monster = _attackMonsters.OrderBy((monster) => Player.Position.DistanceTo(monster.Position)).FirstOrDefault();
        if (monster == null) { return; }

        _canAttackNormal = false;
        monster.Damage(1);

        var points = new[] { Player.ToLocal(Player.Position), Player.ToLocal(monster.Position) };
        var line = new Line2D { Width = 1, DefaultColor = Player.Color, Points = points };
        Player.AddChild(line);
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
