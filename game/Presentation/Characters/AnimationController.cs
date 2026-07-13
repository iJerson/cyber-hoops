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

        UpdateLayerWeights(stats, dt);

        // --- Layer 0: locomotion ---
        var legL = swing * stats.LegSwing * stride;
        var legR = -swing * stats.LegSwing * stride;
        var armL = -swing * stats.ArmSwing * stride;
        var armR = swing * stats.ArmSwing * stride;
        var lean = -stats.SprintLean * stride;
        var bob = Mathf.Abs(Mathf.Sin(gaitAngle * 2f)) * stats.BobHeight * stride;
        var breathe = (1f - stride) * stats.BreatheAmplitude * Mathf.Sin(_idleTime * stats.BreatheRate);
        var pelvisYaw = 0f;
        var crouch = 0f;

        // --- Layer 1: dribble (arms + posture) ---
        var dribbleWeight = _dribbleWeight * (1f - _armsRaised);
        if (dribbleWeight > 0f)
        {
            var pump = stats.DribblePumpBase + stats.DribblePumpRange * Dribble!.NormalizedBallHeight;
            armR = Mathf.Lerp(armR, pump, dribbleWeight);
            lean += -stats.DribbleLean * dribbleWeight;
            crouch += stats.DribbleCrouch * dribbleWeight;

            if (_protectWeight > 0f)
            {
                var protect = _protectWeight * dribbleWeight;
                pelvisYaw = stats.ProtectYaw * Dribble.ProtectSideSign * protect;
                crouch += stats.ProtectCrouch * protect;
                armL = Mathf.Lerp(armL, stats.ShieldArmPitch, protect);
            }
        }

        // --- Stop state + action pre-load: extra sink ---
        crouch += stats.StopSink * _stopWeight;
        crouch += stats.DipCrouch * _dip;

        // --- Layer 2: action override (arms overhead) ---
        armL = Mathf.Lerp(armL, stats.ArmsRaisedAngle, _armsRaised);
        armR = Mathf.Lerp(armR, stats.ArmsRaisedAngle, _armsRaised);

        // --- Knees: baseline flexion (never tall), swing-phase bend lagging the
        // hip, plus bend that visually explains any crouch. Feet counter-rotate
        // to stay level with the floor. Knee axis: positive X kicks the shin back.
        var kneeCrouch = crouch * stats.KneeCrouchGain;
        var kneeL = stats.KneeBaseline + kneeCrouch
                    + Mathf.Max(0f, Mathf.Sin(gaitAngle + stats.KneePhaseOffset)) * stats.KneeSwing * stride;
        var kneeR = stats.KneeBaseline + kneeCrouch
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
