using CyberHoops.Core.Character;
using Xunit;

namespace CyberHoops.Tests.Core;

public class ArmIKTests
{
    private const double L1 = 0.27;
    private const double L2 = 0.315;

    private static (double Y, double Z) HandFromPose(ArmPose pose, double l1, double l2)
    {
        // Forward kinematics matching the project convention: rotate the
        // upper arm by shoulderPitch, then rotate the forearm by the
        // additional (shoulderPitch + elbowFlexion) about the same axis.
        var elbowY = -l1 * Math.Cos(pose.ShoulderPitch);
        var elbowZ = -l1 * Math.Sin(pose.ShoulderPitch);
        var total = pose.ShoulderPitch + pose.ElbowFlexion;
        var handY = elbowY - l2 * Math.Cos(total);
        var handZ = elbowZ - l2 * Math.Sin(total);
        return (handY, handZ);
    }

    [Fact]
    public void FullyExtendedStraightDown_NoBendNoPitch()
    {
        var pose = ArmIK.Solve(-(L1 + L2), 0.0, L1, L2);

        Assert.Equal(0.0, pose.ShoulderPitch, 6);
        Assert.Equal(0.0, pose.ElbowFlexion, 6);
    }

    [Theory]
    [InlineData(-0.3, -0.2)]
    [InlineData(-0.45, -0.3)]
    [InlineData(-0.1, -0.5)]
    [InlineData(-0.45, -0.05)]
    public void Solve_HandLandsOnReachableTarget(double targetY, double targetZ)
    {
        var pose = ArmIK.Solve(targetY, targetZ, L1, L2);

        var (handY, handZ) = HandFromPose(pose, L1, L2);

        Assert.Equal(targetY, handY, 4);
        Assert.Equal(targetZ, handZ, 4);
    }

    [Fact]
    public void ElbowFlexion_IsNeverNegative()
    {
        foreach (var (y, z) in new[] { (-0.3, -0.2), (-0.5, -0.4), (-0.1, -0.5), (-(L1 + L2), 0.0) })
        {
            var pose = ArmIK.Solve(y, z, L1, L2);
            Assert.True(pose.ElbowFlexion >= -1e-9, $"elbow flexion {pose.ElbowFlexion} went negative for ({y},{z})");
        }
    }

    [Fact]
    public void UnreachableTarget_ClampsToMaxReach()
    {
        // Way beyond L1+L2 in the same direction as a reachable point.
        var farPose = ArmIK.Solve(-(L1 + L2) * 5.0, 0.0, L1, L2);
        var nearPose = ArmIK.Solve(-(L1 + L2), 0.0, L1, L2);

        Assert.Equal(nearPose.ShoulderPitch, farPose.ShoulderPitch, 6);
        Assert.Equal(nearPose.ElbowFlexion, farPose.ElbowFlexion, 6);
    }

    [Fact]
    public void TargetAtShoulder_DoesNotThrowAndStaysReachable()
    {
        var pose = ArmIK.Solve(0.0, 0.0, L1, L2);

        Assert.True(double.IsFinite(pose.ShoulderPitch));
        Assert.True(double.IsFinite(pose.ElbowFlexion));
    }

    [Fact]
    public void Solve_IsDeterministic()
    {
        var a = ArmIK.Solve(-0.3, -0.4, L1, L2);
        var b = ArmIK.Solve(-0.3, -0.4, L1, L2);

        Assert.Equal(a, b);
    }

    [Fact]
    public void NonPositiveLengths_Throw()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ArmIK.Solve(-0.3, -0.2, 0.0, L2));
        Assert.Throws<ArgumentOutOfRangeException>(() => ArmIK.Solve(-0.3, -0.2, L1, 0.0));
    }
}
