namespace leveling.player.job_state;

using System.Threading.Tasks;
using Godot;
using lib.custom_nodes.circle2d;
using lib.extensions;

public partial class MagicianState : JobState
{
    private bool _canAttackNormal = true;
    private bool _canAttackArea = true;

    public override void Ready()
    {
        this.BindNodes();
    }

    public override void Update(double delta)
    {
    }

    public override void PhysicsUpdate(double delta)
    {
        if (_canAttackNormal && Input.IsActionJustPressed("button_y")) { AttackNormal(); }
        if (_canAttackArea && Input.IsActionJustPressed("button_b")) { AttackArea(); }
    }

    private async Task AttackNormal()
    {
        var monster = Player.AttackMonster(9);
        if (monster == null) { return; }

        _canAttackNormal = false;

        // ユピテルサンダー
        8.TimesAsync(async (i) =>
        {
            monster.Damage(1);
            var points = new[] { Player.ToLocal(Player.Position), Player.ToLocal(monster.Position) };
            var line = new Line2D { Width = 1, DefaultColor = Player.Color, Points = points };
            Player.AddChild(line);
            await this.WaitSeconds(0.1f);
            line.QueueFree();
        });

        await this.WaitSeconds(1.0f);
        _canAttackNormal = true;
    }

    private async Task AttackArea()
    {
        _canAttackArea = false;
        var circle = new Circle2D { Size = Player.CellSize * 9 * 2, Color = new Color(1, 1, 1, 0.3f), IsFilled = true };
        Player.AddChild(circle);

        var monsters = Player.AttackAreaMonsters(9);
        foreach (var monster in monsters)
        {
            monster.Damage(1);
        }
        await this.WaitSeconds(0.1f);
        Player.RemoveChild(circle);

        await this.WaitSeconds(2.0f);
        _canAttackArea = true;
    }
}
