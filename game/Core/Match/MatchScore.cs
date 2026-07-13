namespace CyberHoops.Core.Match;

/// <summary>
/// Score for a two-team match played to a target (first to 21). Engine-free
/// and deterministic; the match manager feeds it events and reads the winner.
/// </summary>
public sealed class MatchScore
{
    private readonly int[] _points = new int[2];

    public int TargetScore { get; }

    /// <summary>Winning team index, or null while the match is live.</summary>
    public int? Winner { get; private set; }

    public MatchScore(int targetScore)
    {
        if (targetScore <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(targetScore), "Target score must be positive.");
        }

        TargetScore = targetScore;
    }

    public int PointsFor(int team)
    {
        ValidateTeam(team);
        return _points[team];
    }

    /// <summary>
    /// Adds points and returns true when this basket wins the match.
    /// Further scoring after a win is rejected.
    /// </summary>
    public bool AddPoints(int team, int points)
    {
        ValidateTeam(team);
        if (points <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(points), "Points must be positive.");
        }

        if (Winner is not null)
        {
            throw new InvalidOperationException("Match is already won.");
        }

        _points[team] += points;
        if (_points[team] >= TargetScore)
        {
            Winner = team;
        }

        return Winner is not null;
    }

    private static void ValidateTeam(int team)
    {
        if (team is not (0 or 1))
        {
            throw new ArgumentOutOfRangeException(nameof(team), "Team must be 0 or 1.");
        }
    }
}
