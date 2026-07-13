using CyberHoops.Core.StateMachine;
using Xunit;

namespace CyberHoops.Tests.Core;

public class StateMachineTests
{
    private sealed class RecordingState : IState
    {
        public string Name { get; }
        public int EnterCount { get; private set; }
        public int ExitCount { get; private set; }
        public double AccumulatedDelta { get; private set; }

        public RecordingState(string name) => Name = name;

        public void Enter() => EnterCount++;
        public void Update(double delta) => AccumulatedDelta += delta;
        public void Exit() => ExitCount++;
    }

    [Fact]
    public void TransitionTo_EntersTargetState()
    {
        var machine = new StateMachine();
        var idle = new RecordingState("Idle");
        machine.AddState(idle);

        machine.TransitionTo("Idle");

        Assert.Same(idle, machine.Current);
        Assert.Equal(1, idle.EnterCount);
    }

    [Fact]
    public void TransitionTo_ExitsPreviousState()
    {
        var machine = new StateMachine();
        var idle = new RecordingState("Idle");
        var move = new RecordingState("Move");
        machine.AddState(idle);
        machine.AddState(move);
        machine.TransitionTo("Idle");

        machine.TransitionTo("Move");

        Assert.Equal(1, idle.ExitCount);
        Assert.Equal(1, move.EnterCount);
        Assert.Same(move, machine.Current);
    }

    [Fact]
    public void TransitionTo_SameState_IsNoOp()
    {
        var machine = new StateMachine();
        var idle = new RecordingState("Idle");
        machine.AddState(idle);
        machine.TransitionTo("Idle");

        machine.TransitionTo("Idle");

        Assert.Equal(1, idle.EnterCount);
        Assert.Equal(0, idle.ExitCount);
    }

    [Fact]
    public void TransitionTo_UnknownState_Throws()
    {
        var machine = new StateMachine();

        Assert.Throws<ArgumentException>(() => machine.TransitionTo("Nope"));
    }

    [Fact]
    public void AddState_DuplicateName_Throws()
    {
        var machine = new StateMachine();
        machine.AddState(new RecordingState("Idle"));

        Assert.Throws<ArgumentException>(() => machine.AddState(new RecordingState("Idle")));
    }

    [Fact]
    public void Update_TicksOnlyActiveState()
    {
        var machine = new StateMachine();
        var idle = new RecordingState("Idle");
        var move = new RecordingState("Move");
        machine.AddState(idle);
        machine.AddState(move);

        machine.Update(1.0); // no active state yet — must not throw
        machine.TransitionTo("Idle");
        machine.Update(0.5);
        machine.TransitionTo("Move");
        machine.Update(0.25);

        Assert.Equal(0.5, idle.AccumulatedDelta);
        Assert.Equal(0.25, move.AccumulatedDelta);
    }

    [Fact]
    public void StateChanged_ReportsPreviousAndNext()
    {
        var machine = new StateMachine();
        machine.AddState(new RecordingState("Idle"));
        machine.AddState(new RecordingState("Move"));
        var events = new List<(string? Previous, string Next)>();
        machine.StateChanged += (prev, next) => events.Add((prev, next));

        machine.TransitionTo("Idle");
        machine.TransitionTo("Move");

        Assert.Equal(new (string?, string)[] { (null, "Idle"), ("Idle", "Move") }, events);
    }
}
