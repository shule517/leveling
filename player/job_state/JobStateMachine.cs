namespace leveling.player.job_state;
using Godot;
using Godot.Collections;

// 参考：https://www.youtube.com/watch?v=Kcg1SEgDqyk
public partial class JobStateMachine : Node
{
    private Dictionary<string, JobState> _states = new();
    private JobState _currentState;

    public override void _Ready()
    {
        foreach (var node in GetChildren())
        {
            GD.Print($"node: {node}");
            if (node is JobState state)
            {
                _states[state.Name] = state;
                state.JobStateMachine = this;
                state.Ready();
                state.Exit();
            }

            _currentState = _states["KnightState"];
            _currentState.Enter();
        }

        GD.Print($"_states: {_states.Keys}");
    }

    public override void _Process(double delta)
    {
        _currentState.Update(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        _currentState.PhysicsUpdate(delta);
    }

    public void TransitionTo(string key)
    {
        if (!_states.ContainsKey(key) || _currentState == _states[key]) { return; }

        _currentState.Exit();
        _currentState = _states[key];
        _currentState.Enter();
    }
}
