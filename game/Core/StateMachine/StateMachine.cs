namespace CyberHoops.Core.StateMachine;

/// <summary>
/// Minimal deterministic finite state machine. States are registered by name;
/// transitions are explicit via <see cref="TransitionTo"/>. Engine-agnostic so
/// the same machine drives player locomotion now and AI behaviour in M4.
/// </summary>
public sealed class StateMachine
{
    private readonly Dictionary<string, IState> _states = new();

    public IState? Current { get; private set; }

    /// <summary>Raised after a transition completes, with (previous, next) state names.</summary>
    public event Action<string?, string>? StateChanged;

    public void AddState(IState state)
    {
        ArgumentNullException.ThrowIfNull(state);
        if (!_states.TryAdd(state.Name, state))
        {
            throw new ArgumentException($"State '{state.Name}' is already registered.", nameof(state));
        }
    }

    public bool HasState(string name) => _states.ContainsKey(name);

    /// <summary>
    /// Switches to the named state, calling Exit on the current state and Enter
    /// on the next. Transitioning to the already-active state is a no-op.
    /// </summary>
    public void TransitionTo(string name)
    {
        if (!_states.TryGetValue(name, out var next))
        {
            throw new ArgumentException($"Unknown state '{name}'.", nameof(name));
        }

        if (ReferenceEquals(Current, next))
        {
            return;
        }

        var previous = Current?.Name;
        Current?.Exit();
        Current = next;
        next.Enter();
        StateChanged?.Invoke(previous, name);
    }

    /// <summary>Ticks the active state. Safe to call before any transition.</summary>
    public void Update(double delta) => Current?.Update(delta);
}
