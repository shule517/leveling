namespace leveling.player.job_state;

using Godot;

public partial class JobState : Node
{
    public JobStateMachine JobStateMachine { get; set; } = null!;
    public virtual void Enter() { }
    public virtual void Exit() { }

    public virtual void Ready() { }
    public virtual void Update(double delta) { }
    public virtual void PhysicsUpdate(double delta) { }
}
