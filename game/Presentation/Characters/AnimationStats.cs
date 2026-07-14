using Godot;

namespace CyberHoops.Presentation.Characters;

/// <summary>
/// Every pose magnitude and blend speed for the procedural animator, sim
/// baseline and arcade overrides together. Designers tune the .tres; no
/// values in code.
/// </summary>
[GlobalClass]
public partial class AnimationStats : Resource
{
    // --- Locomotion (layer 0) ---

    /// <summary>Leg swing amplitude at full run, radians.</summary>
    [Export] public float LegSwing { get; set; } = 0.7f;

    /// <summary>Arm swing amplitude at full run, radians.</summary>
    [Export] public float ArmSwing { get; set; } = 0.55f;

    /// <summary>Vertical body bob at full run, metres.</summary>
    [Export] public float BobHeight { get; set; } = 0.05f;

    /// <summary>Baseline knee flexion when live — the "never tall" rule, radians.</summary>
    [Export] public float KneeBaseline { get; set; } = 0.15f;

    /// <summary>Knee flexion amplitude during the swing phase at full run, radians.</summary>
    [Export] public float KneeSwing { get; set; } = 0.9f;

    /// <summary>Gait-phase offset of peak knee flexion after the hip swing, radians.</summary>
    [Export] public float KneePhaseOffset { get; set; } = 1.6f;

    /// <summary>Extra knee flexion per metre of crouch, radians/m.</summary>
    [Export] public float KneeCrouchGain { get; set; } = 3.0f;

    /// <summary>How much the feet counter-rotate to stay level with the ground, 0–1.</summary>
    [Export] public float FootLevelFactor { get; set; } = 0.7f;

    /// <summary>Forward torso lean at full sprint, radians (arcade-exaggerated).</summary>
    [Export] public float SprintLean { get; set; } = 0.3f;

    /// <summary>Idle breathing sway amplitude, metres.</summary>
    [Export] public float BreatheAmplitude { get; set; } = 0.012f;

    /// <summary>Idle breathing rate, radians/s.</summary>
    [Export] public float BreatheRate { get; set; } = 2.2f;

    /// <summary>Normalized speed above which breathing fully yields to gait bob (one oscillator at a time).</summary>
    [Export] public float BreatheStrideCutoff { get; set; } = 0.3f;

    /// <summary>Arm swing lags the leg swing by this phase, radians (overlap — arms trail).</summary>
    [Export] public float ArmPhaseLag { get; set; } = 0.25f;

    /// <summary>Hip pitch of the tucked legs during dunk flight, radians.</summary>
    [Export] public float FlightLegTuck { get; set; } = 0.45f;

    /// <summary>Extra knee flexion during dunk flight, radians.</summary>
    [Export] public float FlightKneeTuck { get; set; } = 1.1f;

    // --- Dribble layer (layer 1) ---

    /// <summary>Crouch depth while dribbling, metres.</summary>
    [Export] public float DribbleCrouch { get; set; } = 0.18f;

    /// <summary>
    /// Extra crouch at full sprint while dribbling, metres — brings the
    /// shoulder closer to the lower, faster running bounce so the arm's IK
    /// target stays within reach more of the cycle (matches the research
    /// library's low forward-driving stance).
    /// </summary>
    [Export] public float SprintDribbleCrouch { get; set; } = 0.08f;

    /// <summary>
    /// Extra knee flexion synced to the ball's own bounce phase while
    /// standing and dribbling, radians — without this the stride-driven knee
    /// swing is exactly zero at rest and the legs go dead-static even though
    /// the arm is clearly moving. Peaks at floor contact, eases at the catch.
    /// Fades out as speed rises (gait swing takes over — one motion source
    /// at a time, same rule as the breathing/bob handoff).
    /// </summary>
    [Export] public float DribbleLifeKnee { get; set; } = 0.18f;

    /// <summary>Extra pelvis sink synced to the ball's bounce phase while standing and dribbling, metres.</summary>
    [Export] public float DribbleLifeBob { get; set; } = 0.02f;

    /// <summary>Forward lean while dribbling, radians.</summary>
    [Export] public float DribbleLean { get; set; } = 0.12f;

    /// <summary>
    /// Upper arm length (shoulder to elbow), metres — must match the rig's
    /// ElbowPivot offset. The dribble arm is posed by 2-bone IK (see
    /// CyberHoops.Core.Character.ArmIK) so shoulder pitch and elbow flexion
    /// always land the hand on the ball's real position — no more hand-tuned
    /// angle curves to rebalance every time the rig changes.
    /// </summary>
    [Export] public float UpperArmLength { get; set; } = 0.27f;

    /// <summary>Forearm length (elbow to hand), metres — must match the rig's Hand offset.</summary>
    [Export] public float ForearmLength { get; set; } = 0.315f;

    /// <summary>Shoulder height above the pelvis, metres — must match the rig's ShoulderPivot Y offset.</summary>
    [Export] public float ShoulderHeightAbovePelvis { get; set; } = 0.48f;

    /// <summary>Pelvis yaw in the protect stance, radians (ball side turned away).</summary>
    [Export] public float ProtectYaw { get; set; } = 0.9f;

    /// <summary>Extra crouch in the protect stance, metres.</summary>
    [Export] public float ProtectCrouch { get; set; } = 0.06f;

    /// <summary>Free-arm shield bar pitch in the protect stance, radians.</summary>
    [Export] public float ShieldArmPitch { get; set; } = 0.7f;

    /// <summary>Elbow flexion at rest — a straight-locked elbow reads unnatural, radians.</summary>
    [Export] public float ElbowBaseline { get; set; } = 0.2f;

    /// <summary>Extra elbow flexion at the rear of the gait swing, radians.</summary>
    [Export] public float ElbowGaitSwing { get; set; } = 0.5f;

    /// <summary>Elbow flexion in the protect shield bar, radians (~90°).</summary>
    [Export] public float ElbowShieldBend { get; set; } = 1.4f;

    /// <summary>Elbow flexion with arms overhead (dunk carry) — near straight, radians.</summary>
    [Export] public float ElbowRaisedBend { get; set; } = 0.15f;

    // --- Action layer (layer 2) ---

    /// <summary>Shoulder pitch with arms fully overhead (dunk carry), radians.</summary>
    [Export] public float ArmsRaisedAngle { get; set; } = 2.7f;

    /// <summary>Arms raise/lower blend speed, per second.</summary>
    [Export] public float ArmsRaiseSpeed { get; set; } = 10.0f;

    /// <summary>Crouch depth during an action pre-load dip, metres.</summary>
    [Export] public float DipCrouch { get; set; } = 0.12f;

    /// <summary>Dip blend speed, per second (fast — the dip itself is 50–70ms).</summary>
    [Export] public float DipBlendSpeed { get; set; } = 20.0f;

    // --- Stop state ---

    /// <summary>Extra pelvis sink during a hard stop, metres.</summary>
    [Export] public float StopSink { get; set; } = 0.1f;

    // --- Blending ---

    /// <summary>Generic layer weight blend speed, per second.</summary>
    [Export] public float LayerBlendSpeed { get; set; } = 8.0f;

    // --- Head (layer 4) ---

    /// <summary>Max head yaw toward the focus target, radians.</summary>
    [Export] public float HeadYawLimit { get; set; } = 0.7f;

    /// <summary>Head aim blend speed, per second.</summary>
    [Export] public float HeadTurnSpeed { get; set; } = 6.0f;
}
