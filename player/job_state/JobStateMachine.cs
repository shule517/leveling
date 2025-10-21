namespace leveling.player.job_state;

using System.Linq;
using Godot;
using Godot.Collections;

// 参考：https://www.youtube.com/watch?v=Kcg1SEgDqyk
public partial class JobStateMachine : Node {
    private JobState _currentState = null!;
    private Dictionary<string, JobState> _states = new();
    [Export] public NodePath InitialState = null!;

    public override void _Ready() {
        foreach (var node in GetChildren()) {
            if (node is JobState state) {
                _states[state.Name] = state;
                state.JobStateMachine = this;
                state.Ready();
                state.Exit();
            }
        }

        var currentStateName = InitialState.ToString();
        _currentState = _states[currentStateName];
        _currentState.Enter();
    }

    public override void _Process(double delta) {
        _currentState.Update(delta);
    }

    public override void _PhysicsProcess(double delta) {
        if (Input.IsActionJustPressed("button_zl")) {
            var keys = _states.Keys.ToList();
            var currentIndex = keys.IndexOf(_currentState.Name);
            var prevIndex = (currentIndex - 1 + keys.Count) % keys.Count; // 先頭の前なら末尾へ
            _currentState = _states[keys[prevIndex]];
            GD.Print($"_currentState: {_currentState.Name}");
        }

        if (Input.IsActionJustPressed("button_zr")) {
            var keys = _states.Keys.ToList();
            var currentIndex = keys.IndexOf(_currentState.Name);
            var nextIndex = (currentIndex + 1) % keys.Count; // 最後なら先頭に戻る
            _currentState = _states[keys[nextIndex]];
            GD.Print($"_currentState: {_currentState.Name}");
        }

        _currentState.PhysicsUpdate(delta);
    }

    public void TransitionTo(string key) {
        if (!_states.TryGetValue(key, out var value) || _currentState == value) { return; }

        _currentState.Exit();
        _currentState = value;
        _currentState.Enter();
    }
}
