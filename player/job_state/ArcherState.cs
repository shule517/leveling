namespace leveling.player.job_state;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using lib.attributes;
using lib.custom_nodes.circle2d;
using lib.extensions;
using monster;

public partial class ArcherState : JobState
{
    [Node] private Area2D _attackNormalArea2D = null!;

    private bool _canAttackNormal = true;
    private bool _canAttackArea = true;

    public override void Ready()
    {
        this.BindNodes();
        _attackNormalArea2D.BodyEntered += AttackNormalArea2DOnBodyEntered;
        _attackNormalArea2D.BodyExited += AttackNormalArea2DOnBodyExited;
    }

    public override void Update(double delta)
    {
    }

    public override void PhysicsUpdate(double delta)
    {
        if (_canAttackNormal && Input.IsActionJustPressed("button_y")) { SingleAttack(); }
        if (_canAttackArea && Input.IsActionJustPressed("button_b")) { DoubleAttack(); }
    }

    private async Task SingleAttack()
    {
        var monster = _attackMonsters.OrderBy((monster) => Player.Position.DistanceTo(monster.Position)).FirstOrDefault();
        if (monster == null) { return; }

        _canAttackNormal = false;

        // TODO: 本来のDAは確率50%。
        // TODO: レベルアップで5%からだんだん上がっていって、最後は常にでもいいかもしれない
        // var attackCount = GD.Randf() < 1.0f ? 2 : 1;
        var attackCount = 1;
        attackCount.TimesAsync(async (i) => await Attack(monster));
        await this.WaitSeconds(1.0f);
        _canAttackNormal = true;
    }

    // TODO: スキル自体をクラス化したいね
    // TODO: そもそも射程などを数値で管理する
    // TODO: 1セル = 16pxを定義して、9セルなら 16 * 9 = 136px
    private async Task DoubleAttack()
    {
        var monster = _attackMonsters.OrderBy((monster) => Player.Position.DistanceTo(monster.Position)).FirstOrDefault();
        if (monster == null) { return; }

        _canAttackNormal = false;

        // TODO: 本来のDAは確率50%。
        // TODO: レベルアップで5%からだんだん上がっていって、最後は常にでもいいかもしれない
        var attackCount = GD.Randf() < 1.0f ? 2 : 1;
        attackCount.TimesAsync(async (i) => await Attack(monster));
        await this.WaitSeconds(1.0f);
        _canAttackNormal = true;
    }

    private async Task Attack(Monster? monster)
    {
        monster.Damage(1);

        // 攻撃の描画
        var points = new[] { Player.ToLocal(Player.Position), Player.ToLocal(monster.Position) };
        var line = new Line2D { Width = 1, DefaultColor = Player.Color, Points = points };
        Player.AddChild(line);
        await this.WaitSeconds(0.1f);
        line.QueueFree();
    }

    private async Task AttackArea()
    {
        _canAttackArea = false;
        var circle = new Circle2D { Size = 200, Color = new Color(1, 1, 1, 0.3f), IsFilled = true };
        Player.AddChild(circle);
        foreach (var monster in _attackMonsters)
        {
            // TODO: ノックバックをMonsterの実装に依存しているのを スキル依存にする必要がある
            monster.Damage(1);
        }
        await this.WaitSeconds(0.1f);
        Player.RemoveChild(circle);

        await this.WaitSeconds(2.0f);
        _canAttackArea = true;
    }

    private readonly List<Monster> _attackMonsters = new();
    private void AttackNormalArea2DOnBodyEntered(Node2D body)
    {
        if (body is Monster monster) { _attackMonsters.Add(monster); }
    }

    private void AttackNormalArea2DOnBodyExited(Node2D body)
    {
        if (body is Monster monster) { _attackMonsters.Remove(monster); }
    }
}
