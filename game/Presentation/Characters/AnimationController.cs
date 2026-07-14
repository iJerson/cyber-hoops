using CyberHoops.Core.Character;
using Godot;
using CyberHoops.Gameplay.Player;

namespace CyberHoops.Presentation.Characters;

/// <summary>
/// The only writer to the rig. Layered procedural poses per the blend-tree
/// design: locomotion base (gait from Movement's master clock), dribble layer
/// (arm pump synced to the ball's real bounce, crouch/lean, protect shield),
/// action layer (arms overhead for dunks) and head look-at. All magnitudes in
/// <see cref="AnimationStats"/>. Reads gameplay state; never mutates it.
/// </summary>
[GlobalClass]
public partial class AnimationController : Node
{
    [Export] public CharacterRig? Rig { get; set; }
    [Export] public MovementComponent? Movement { get; set; }
    [Export] public DribbleComponent? Dribble { get; set; }
    [Export] public CharacterStateMachine? StateMachine { get; set; }
    [Export] public AnimationStats? Stats { get; set; }

    /// <summary>Group of the node the head looks at (the hoop).</summary>
    [Export] public StringName FocusGroup { get; set; } = "shot_target";

    private float _armsRaisedTarget;
    private float _armsRaised;
    private float _dipTarget;
    private float _dip;
    private float _dribbleWeight;
    private float _protectWeight;
    private float _stopWeight;
    private float _protectSide = 1f;
    private float _idleTime;
    private float _headYaw;
    private float _pelvisRestY;

    /// <summary>0 = arms follow gait, 1 = both arms overhead (dunk carry).</summary>
    public void SetArmsRaised(float amount) => _armsRaisedTarget = Mathf.Clamp(amount, 0f, 1f);

    /// <summary>Pre-load dip for explosive actions: 1 = full anticipation crouch.</summary>
    public void SetActionDip(float amount) => _dipTarget = Mathf.Clamp(amount, 0f, 1f);

    public override void _Ready()
    {
        if (Rig is null || Movement is null || Dribble is null || StateMachine is null || Stats is null)
        {
            GD.PushError($"{nameof(AnimationController)} is missing required exports.");
            SetPhysicsProcess(false);
            return;
        }

        if (Rig.Pelvis is not null)
        {
            _pelvisRestY = Rig.Pelvis.Position.Y;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        var rig = Rig!;
        if (rig.Pelvis is null || rig.ShoulderPivotL is null || rig.ShoulderPivotR is null
            || rig.HipPivotL is null || rig.HipPivotR is null)
        {
            return;
        }

        var stats = Stats!;
        var dt = (float)delta;
        _idleTime += dt;

        var stride = Movement!.NormalizedSpeed;
        var gaitAngle = Movement.GaitPhase * Mathf.Tau;
        var swing = Mathf.Sin(gaitAngle);
        // Arms trail the legs slightly (overlap) instead of exact antiphase.
        var armSwing = Mathf.Sin(gaitAngle - stats.ArmPhaseLag);

        UpdateLayerWeights(stats, dt);

        // --- Layer 0: locomotion ---
        var legL = swing * stats.LegSwing * stride;
        var legR = -swing * stats.LegSwing * stride;
        var armL = -armSwing * stats.ArmSwing * stride;
        var armR = armSwing * stats.ArmSwing * stride;
        var lean = -stats.SprintLean * stride;
        // Two bobs per stride cycle, minima at the foot plants (0 / 0.5).
        var bob = Mathf.Abs(Mathf.Sin(gaitAngle)) * stats.BobHeight * stride;
        // One pelvis oscillator at a time: breathing gates off as gait bob takes over.
        var breatheWeight = Mathf.Clamp(1f - stride / stats.BreatheStrideCutoff, 0f, 1f);
        var breathe = breatheWeight * stats.BreatheAmplitude * Mathf.Sin(_idleTime * stats.BreatheRate);
        var pelvisYaw = 0f;
        var crouch = 0f;

        // Elbow baseline: a dead-straight elbow reads unnatural even at rest.
        // Bends more at the rear of the swing (natural running arm carry).
        var elbowGaitL = stats.ElbowBaseline + stats.ElbowGaitSwing * stride * Mathf.Max(0f, -armSwing);
        var elbowGaitR = stats.ElbowBaseline + stats.ElbowGaitSwing * stride * Mathf.Max(0f, armSwing);
        var elbowL = elbowGaitL;
        var elbowR = elbowGaitR;

        // --- Layer 1: dribble (arms + posture) ---
        var dribbleWeight = _dribbleWeight * (1f - _armsRaised);
        if (dribbleWeight > 0f)
        {
            // 2-bone IK: the hand always lands on the ball's real position
            // (relative to the shoulder), so shoulder pitch and elbow flexion
            // come out of the actual geometry instead of two independently
            // tuned angle curves that drift out of sync whenever bone
            // lengths change.
            var targetY = Dribble!.BallHeightAboveFloor - (rig.Pelvis.Position.Y + stats.ShoulderHeightAbovePelvis);
            var targetZ = Dribble.AnchorLocalZ;
            var ik = ArmIK.Solve(targetY, targetZ, stats.UpperArmLength, stats.ForearmLength);
            armR = Mathf.Lerp(armR, (float)ik.ShoulderPitch, dribbleWeight);
            elbowR = Mathf.Lerp(elbowR, (float)ik.ElbowFlexion, dribbleWeight);
            lean += -stats.DribbleLean * dribbleWeight;
            crouch += stats.DribbleCrouch * dribbleWeight;

            if (_protectWeight > 0f)
            {
                var protect = _protectWeight * dribbleWeight;
                // Side sign is blended, not raw — a defender crossing the midline
                // must not snap the pelvis 1.8 rad in one frame.
                pelvisYaw = stats.ProtectYaw * _protectSide * protect;
                crouch += stats.ProtectCrouch * protect;
                armL = Mathf.Lerp(armL, stats.ShieldArmPitch, protect);
                elbowL = Mathf.Lerp(elbowL, stats.ElbowShieldBend, protect);
            }
        }

        // --- Stop state + action pre-load: extra sink ---
        crouch += stats.StopSink * _stopWeight;
        crouch += stats.DipCrouch * _dip;

        // --- Layer 2: action override (arms overhead) ---
        armL = Mathf.Lerp(armL, stats.ArmsRaisedAngle, _armsRaised);
        armR = Mathf.Lerp(armR, stats.ArmsRaisedAngle, _armsRaised);
        elbowL = Mathf.Lerp(elbowL, stats.ElbowRaisedBend, _armsRaised);
        elbowR = Mathf.Lerp(elbowR, stats.ElbowRaisedBend, _armsRaised);

        // --- Dunk flight: legs tuck instead of freezing in the last stride pose ---
        if (_armsRaised > 0f)
        {
            legL = Mathf.Lerp(legL, stats.FlightLegTuck, _armsRaised);
            legR = Mathf.Lerp(legR, stats.FlightLegTuck, _armsRaised);
        }

        // --- Knees: baseline flexion (never tall), swing-phase bend lagging the
        // hip, plus bend that visually explains any crouch. Feet counter-rotate
        // to stay level with the floor. Knee axis: positive X kicks the shin back.
        var kneeCrouch = crouch * stats.KneeCrouchGain;
        var kneeTuck = stats.FlightKneeTuck * _armsRaised;
        var kneeL = stats.KneeBaseline + kneeCrouch + kneeTuck
                    + Mathf.Max(0f, Mathf.Sin(gaitAngle + stats.KneePhaseOffset)) * stats.KneeSwing * stride;
        var kneeR = stats.KneeBaseline + kneeCrouch + kneeTuck
                    + Mathf.Max(0f, Mathf.Sin(gaitAngle + Mathf.Pi + stats.KneePhaseOffset)) * stats.KneeSwing * stride;

        // --- Write channels ---
        rig.HipPivotL.Rotation = new Vector3(legL, 0f, 0f);
        rig.HipPivotR.Rotation = new Vector3(legR, 0f, 0f);
        if (rig.KneePivotL is not null && rig.KneePivotR is not null)
        {
            rig.KneePivotL.Rotation = new Vector3(kneeL, 0f, 0f);
            rig.KneePivotR.Rotation = new Vector3(kneeR, 0f, 0f);
            LevelFoot(rig.KneePivotL, legL + kneeL, stats.FootLevelFactor);
            LevelFoot(rig.KneePivotR, legR + kneeR, stats.FootLevelFactor);
        }
        rig.ShoulderPivotL.Rotation = new Vector3(armL, 0f, 0f);
        rig.ShoulderPivotR.Rotation = new Vector3(armR, 0f, 0f);
        if (rig.ElbowPivotL is not null && rig.ElbowPivotR is not null)
        {
            rig.ElbowPivotL.Rotation = new Vector3(elbowL, 0f, 0f);
            rig.ElbowPivotR.Rotation = new Vector3(elbowR, 0f, 0f);
        }
        rig.Pelvis.Position = rig.Pelvis.Position with { Y = _pelvisRestY + bob + breathe - crouch };
        rig.Pelvis.Rotation = new Vector3(lean, pelvisYaw, 0f);

        // --- Layer 4: head look-at (always wins the head) ---
        UpdateHead(stats, dt);
    }

    private void UpdateLayerWeights(AnimationStats stats, float dt)
    {
        _armsRaised = Mathf.MoveToward(_armsRaised, _armsRaisedTarget, stats.ArmsRaiseSpeed * dt);
        _dip = Mathf.MoveToward(_dip, _dipTarget, stats.DipBlendSpeed * dt);
        _dribbleWeight = Mathf.MoveToward(_dribbleWeight, Dribble!.IsDribbling ? 1f : 0f, stats.LayerBlendSpeed * dt);
        _protectWeight = Mathf.MoveToward(_protectWeight, Dribble.IsProtecting ? 1f : 0f, stats.LayerBlendSpeed * dt);
        _stopWeight = Mathf.MoveToward(
            _stopWeight,
            StateMachine!.CurrentState == LocomotionStates.Stop ? 1f : 0f,
            stats.LayerBlendSpeed * dt);
        _protectSide = Mathf.MoveToward(_protectSide, Dribble.ProtectSideSign, stats.LayerBlendSpeed * dt);
    }

    /// <summary>Counter-rotates the foot under a knee pivot so the sole stays near-level.</summary>
    private static void LevelFoot(Node3D kneePivot, float legChainPitch, float levelFactor)
    {
        if (kneePivot.GetNodeOrNull<Node3D>("Foot") is { } foot)
        {
            foot.Rotation = new Vector3(-legChainPitch * levelFactor, 0f, 0f);
        }
    }

    private void UpdateHead(AnimationStats stats, float dt)
    {
        if (Rig!.Head is null)
        {
            return;
        }

        var targetYaw = 0f;
        if (GetTree().GetFirstNodeInGroup(FocusGroup) is Node3D focus)
        {
            var local = Rig.Head.GetParentNode3D()!.ToLocal(focus.GlobalPosition);
            var flat = local with { Y = 0f };
            if (flat.LengthSquared() > 0.0001f)
            {
                // Rig faces -Z; yaw toward the focus, clamped.
                targetYaw = Mathf.Clamp(Mathf.Atan2(-flat.X, -flat.Z), -stats.HeadYawLimit, stats.HeadYawLimit);
            }
        }

        _headYaw = Mathf.MoveToward(_headYaw, targetYaw, stats.HeadTurnSpeed * dt);
        Rig.Head.Rotation = new Vector3(0f, _headYaw, 0f);
    }
}
