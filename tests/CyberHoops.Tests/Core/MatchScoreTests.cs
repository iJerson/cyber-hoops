using CyberHoops.Core.Match;
using Xunit;

namespace CyberHoops.Tests.Core;

public class MatchScoreTests
{
    [Fact]
    public void NewMatch_ZeroZero_NoWinner()
    {
        var score = new MatchScore(21);

        Assert.Equal(0, score.PointsFor(0));
        Assert.Equal(0, score.PointsFor(1));
        Assert.Null(score.Winner);
    }

    [Fact]
    public void AddPoints_Accumulates()
    {
        var score = new MatchScore(21);

        score.AddPoints(0, 2);
        score.AddPoints(0, 3);
        score.AddPoints(1, 2);

        Assert.Equal(5, score.PointsFor(0));
        Assert.Equal(2, score.PointsFor(1));
    }

    [Fact]
    public void ReachingTarget_Wins()
    {
        var score = new MatchScore(4);
        score.AddPoints(1, 2);

        var won = score.AddPoints(1, 2);

        Assert.True(won);
        Assert.Equal(1, score.Winner);
    }

    [Fact]
    public void ExceedingTarget_Wins()
    {
        var score = new MatchScore(4);
        score.AddPoints(0, 3);

        var won = score.AddPoints(0, 3);

        Assert.True(won);
        Assert.Equal(0, score.Winner);
    }

    [Fact]
    public void ScoringAfterWin_Throws()
    {
        var score = new MatchScore(2);
        score.AddPoints(0, 2);

        Assert.Throws<InvalidOperationException>(() => score.AddPoints(1, 2));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(2)]
    public void InvalidTeam_Throws(int team)
    {
        var score = new MatchScore(21);

        Assert.Throws<ArgumentOutOfRangeException>(() => score.AddPoints(team, 2));
        Assert.Throws<ArgumentOutOfRangeException>(() => score.PointsFor(team));
    }

    [Fact]
    public void NonPositivePoints_Throw()
    {
        var score = new MatchScore(21);

        Assert.Throws<ArgumentOutOfRangeException>(() => score.AddPoints(0, 0));
    }

    [Fact]
    public void NonPositiveTarget_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MatchScore(0));
    }
}
