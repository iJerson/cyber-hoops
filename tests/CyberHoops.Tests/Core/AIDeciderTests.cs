using CyberHoops.Core.AI;
using Xunit;

namespace CyberHoops.Tests.Core;

public class AIDeciderTests
{
    private static AIWorldSnapshot Snapshot(
        bool ballFree = false,
        bool selfHasBall = false,
        bool opponentHasBall = false,
        double distanceToHoop = 10.0,
        double shootRange = 4.0) =>
        new(ballFree, selfHasBall, opponentHasBall, distanceToHoop, shootRange);

    [Fact]
    public void SelfHasBall_OutOfRange_Attacks()
    {
        Assert.Equal(AIStateNames.Attack, AIDecider.Decide(Snapshot(selfHasBall: true, distanceToHoop: 8.0)));
    }

    [Fact]
    public void SelfHasBall_InRange_Shoots()
    {
        Assert.Equal(AIStateNames.Shoot, AIDecider.Decide(Snapshot(selfHasBall: true, distanceToHoop: 3.5)));
    }

    [Fact]
    public void SelfHasBall_ExactlyAtRange_Shoots()
    {
        Assert.Equal(AIStateNames.Shoot, AIDecider.Decide(Snapshot(selfHasBall: true, distanceToHoop: 4.0)));
    }

    [Fact]
    public void BallFree_Chases()
    {
        Assert.Equal(AIStateNames.ChaseBall, AIDecider.Decide(Snapshot(ballFree: true)));
    }

    [Fact]
    public void OpponentHasBall_Defends()
    {
        Assert.Equal(AIStateNames.Defend, AIDecider.Decide(Snapshot(opponentHasBall: true)));
    }

    [Fact]
    public void NothingHappening_Idles()
    {
        Assert.Equal(AIStateNames.Idle, AIDecider.Decide(Snapshot()));
    }

    [Fact]
    public void OwnPossession_BeatsFreeBallFlag()
    {
        // Possession is the stronger signal even if flags disagree for a tick.
        Assert.Equal(AIStateNames.Shoot, AIDecider.Decide(Snapshot(ballFree: true, selfHasBall: true, distanceToHoop: 2.0)));
    }
}
