namespace leveling.player.job_state;

using System.Threading.Tasks;
using Godot;
using lib.extensions;
using monster;

public partial class ArcherState : JobState {
    private bool _canAttackArea = true;
    private bool _canAttackNormal = true;

    public override void Ready() {
        this.BindNodes();
    }

    public override void Update(double delta) {
    }

    public override void PhysicsUpdate(double delta) {
        if (_canAttackNormal && Input.IsActionJustPressed("button_y")) { SingleAttack(); }

        if (_canAttackArea && Input.IsActionJustPressed("button_b")) { DoubleAttack(); }
    }

    private async Task SingleAttack() {
        var monster = Player.AttackMonster(9);
        if (monster == null) { return; }

        _canAttackNormal = false;

        // TODO: 本来のDAは確率50%。
        // TODO: レベルアップで5%からだんだん上がっていって、最後は常にでもいいかもしれない
        // var attackCount = GD.Randf() < 1.0f ? 2 : 1;
        var attackCount = 1;
        attackCount.TimesAsync(async i => await Attack(monster));
        await this.WaitSeconds(1.0f);
        _canAttackNormal = true;
    }

    // TODO: スキル自体をクラス化したいね
    // TODO: そもそも射程などを数値で管理する
    // TODO: 1セル = 16pxを定義して、9セルなら 16 * 9 = 136px
    private async Task DoubleAttack() {
        var monster = Player.AttackMonster(9);
        if (monster == null) { return; }

        _canAttackNormal = false;

        // TODO: 本来のDAは確率50%。
        // TODO: レベルアップで5%からだんだん上がっていって、最後は常にでもいいかもしれない
        var attackCount = GD.Randf() < 1.0f ? 2 : 1;
        attackCount.TimesAsync(async i => await Attack(monster));
        await this.WaitSeconds(1.0f);
        _canAttackNormal = true;
    }

    private async Task Attack(Monster? monster) {
        monster.Damage(1);

        // 攻撃の描画
        var points = new[] { Player.ToLocal(Player.Position), Player.ToLocal(monster.Position) };
        var line = new Line2D { Width = 1, DefaultColor = Player.Color, Points = points };
        Player.AddChild(line);
        await this.WaitSeconds(0.1f);
        line.QueueFree();
    }
}
