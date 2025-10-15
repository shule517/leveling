namespace leveling.player.job_state;
using Godot;

public abstract class JobState : Node
{
    public abstract void OnReady();
    public abstract void OnProcess();
    public abstract void OnPhysicsProcess();
}

public partial class JobStateMachine : Node
{
    private readonly Knight _knight = new();

    public override void _Ready()
    {
        _knight.OnReady();
    }

    public override void _Process(double delta)
    {
        _knight.OnProcess();
    }

    public override void _PhysicsProcess(double delta)
    {
        _knight.OnPhysicsProcess();
    }
}
