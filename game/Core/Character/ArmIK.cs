namespace CyberHoops.Core.Character;

/// <summary>Shoulder pitch and elbow flexion (radians) that place the hand at the target.</summary>
public readonly record struct ArmPose(double ShoulderPitch, double ElbowFlexion);

/// <summary>
/// Analytic 2-bone IK for a shoulder-elbow-hand chain, in the shoulder's local
/// Y-Z plane (X is not solved — the shoulder only pitches). Matches this
/// project's forward-kinematics convention: from a straight hang, a segment
/// of length r rotated by angle a about local X lands at (Y=-r·cos(a),
/// Z=-r·sin(a)); elbow flexion of 0 means the arm is fully extended.
///
/// Ties the dribble arm pose to the ball's real position instead of two
/// independently hand-tuned angle curves (shoulder pump, elbow pump) that
/// have to be re-balanced by hand every time bone lengths or rig proportions
/// change — the recurring bug class this replaces.
/// </summary>
public static class ArmIK
{
    /// <summary>
    /// Solves for the hand at (targetY, targetZ) relative to the shoulder.
    /// Unreachable targets are clamped to the nearest reachable point on the
    /// same ray so the result is always finite and continuous.
    /// </summary>
    public static ArmPose Solve(double targetY, double targetZ, double upperArmLength, double forearmLength)
    {
        if (upperArmLength <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(upperArmLength), "Upper arm length must be positive.");
        }

        if (forearmLength <= 0.0)
        {
            throw new ArgumentOutOfRangeException(nameof(forearmLength), "Forearm length must be positive.");
        }

        var l1 = upperArmLength;
        var l2 = forearmLength;
        var maxReach = l1 + l2;
        var minReach = Math.Abs(l1 - l2);

        var d = Math.Sqrt(targetY * targetY + targetZ * targetZ);
        double y = targetY, z = targetZ;

        if (d < 1e-9)
        {
            // Target sits on the shoulder itself — pick an arbitrary reachable
            // direction (straight down) rather than dividing by zero.
            y = -minReach;
            z = 0.0;
            d = minReach;
        }
        else
        {
            var clampedD = Math.Clamp(d, minReach, maxReach);
            if (Math.Abs(clampedD - d) > 1e-9)
            {
                var scale = clampedD / d;
                y *= scale;
                z *= scale;
                d = clampedD;
            }
        }

        // Guard the algebraic edge (d ≈ 0 only possible when minReach ≈ 0, i.e. l1 ≈ l2).
        d = Math.Max(d, 1e-9);

        var elbowInteriorCos = Math.Clamp((l1 * l1 + l2 * l2 - d * d) / (2.0 * l1 * l2), -1.0, 1.0);
        var elbowFlexion = Math.PI - Math.Acos(elbowInteriorCos);

        var baseAngle = Math.Atan2(-z, -y);
        var alphaCos = Math.Clamp((l1 * l1 + d * d - l2 * l2) / (2.0 * l1 * d), -1.0, 1.0);
        var alpha = Math.Acos(alphaCos);
        var shoulderPitch = baseAngle - alpha;

        return new ArmPose(shoulderPitch, elbowFlexion);
    }
}
