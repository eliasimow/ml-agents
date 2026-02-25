using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

/// <summary>
/// ML-Agents Agent that teaches a configured joint ragdoll to stand up from the ground.
///
/// SETUP REQUIREMENTS:
/// 1. Attach this script to the ragdoll's ROOT body part (e.g. Hips/Pelvis).
/// 2. Assign all ragdoll body parts (Rigidbodies) to the `bodyParts` list in the Inspector.
/// 3. Assign the `hipsTransform` (root bone) and `headTransform`.
/// 4. Set `targetStandingHeight` to match the head's Y position when standing upright.
/// 5. Add a Decision Requester component (Decision Period: 5 recommended).
/// 6. Add a Behavior Parameters component:
///      - Vector Observation Size: auto (calculated below)
///      - Continuous Actions: bodyParts.Count * 3  (x, y, z torque per joint)
/// 7. Each body part Rigidbody must have a ConfigurableJoint.
/// 8. Tag the ground with "Ground".
/// </summary>
public class RagdollStandUpAgent : Agent {
    [Header("Body Parts")]
    [Tooltip("All Rigidbodies in the ragdoll (including root). Order doesn't matter.")]
    public List<Rigidbody> bodyParts = new List<Rigidbody>();

    [Header("Key Transforms")]
    public Transform hipsTransform;   // Root / pelvis
    public Transform headTransform;   // Head bone

    [Header("Standing Parameters")]
    [Tooltip("World-space Y height of head when fully standing.")]
    public float targetStandingHeight = 1.7f;

    [Tooltip("Max torque applied per axis on each joint per step.")]
    public float maxTorque = 200f;

    [Tooltip("Reward multiplier for head height progress.")]
    public float heightRewardScale = 1f;

    [Tooltip("Penalty per step to encourage efficiency.")]
    public float existentialPenalty = -0.001f;

    [Tooltip("Bonus given when agent is considered standing.")]
    public float standingBonus = 1f;

    [Tooltip("Head height threshold to be considered 'standing'.")]
    public float standingHeightThreshold = 0.1f;

    [Header("Episode")]
    [Tooltip("Max steps before episode resets (set in Behavior Parameters too).")]
    public int maxEpisodeSteps = 5000;

    // Cached joint references
    private List<ConfigurableJoint> joints = new List<ConfigurableJoint>();

    // Store initial poses for reset
    private List<Vector3> initialPositions = new List<Vector3>();
    private List<Quaternion> initialRotations = new List<Quaternion>();
    private Vector3 initialHipsPosition;
    private Quaternion initialHipsRotation;

    // Spawn offset randomization
    private readonly float spawnHeightOffset = 0.05f;

    // -----------------------------------------------------------------------
    #region Unity Lifecycle

    public override void Initialize() {
        // Cache joints
        foreach (var rb in bodyParts) {
            var joint = rb.GetComponent<ConfigurableJoint>();
            joints.Add(joint); // null if root / no joint — handled gracefully
            initialPositions.Add(rb.transform.localPosition);
            initialRotations.Add(rb.transform.localRotation);
        }

        initialHipsPosition = hipsTransform.position;
        initialHipsRotation = hipsTransform.rotation;
    }

    #endregion

    // -----------------------------------------------------------------------
    #region Agent Overrides

    public override void OnEpisodeBegin() {
        ResetRagdoll();
    }

    /// <summary>
    /// Observations per body part:
    ///   - local position relative to hips (3)
    ///   - local rotation as quaternion (4)
    ///   - velocity (3)
    ///   - angular velocity (3)
    /// Plus global hips info:
    ///   - hips world position Y (1)
    ///   - hips up dot world up (1)
    ///   - head world position Y (1)
    ///
    /// Total = bodyParts.Count * 13 + 3
    /// Set VectorObservationSize in Behavior Parameters accordingly.
    /// </summary>
    public override void CollectObservations(VectorSensor sensor) {
        // Global orientation cues
        sensor.AddObservation(hipsTransform.position.y);
        sensor.AddObservation(Vector3.Dot(hipsTransform.up, Vector3.up));
        sensor.AddObservation(headTransform.position.y);

        foreach (var rb in bodyParts) {
            // Position relative to hips (normalized)
            Vector3 relativePos = hipsTransform.InverseTransformPoint(rb.transform.position);
            sensor.AddObservation(relativePos);

            // Local rotation as quaternion
            sensor.AddObservation(rb.transform.localRotation);

            // Velocities in hips local space
            sensor.AddObservation(hipsTransform.InverseTransformDirection(rb.linearVelocity));
            sensor.AddObservation(hipsTransform.InverseTransformDirection(rb.angularVelocity));
        }
    }

    /// <summary>
    /// Actions: 3 floats per body part → target angular velocity on x, y, z axes.
    /// Continuous action space size = bodyParts.Count * 3
    /// </summary>
    public override void OnActionReceived(ActionBuffers actions) {
        var continuousActions = actions.ContinuousActions;
        int actionIndex = 0;

        for (int i = 0; i < 1; i++) {
            float tx = continuousActions[actionIndex++];
            float ty = continuousActions[actionIndex++];
            float tz = continuousActions[actionIndex++];

            var rb = bodyParts[i];
            var joint = joints[i];

            if (joint != null) {
                // Drive joints via target velocity (smooth, physically realistic)
                ApplyJointTorque(rb, joint, new Vector3(tx, ty, tz));
            } else {
                // Root body — apply torque directly for orientation control
                rb.AddRelativeTorque(new Vector3(tx, ty, tz) * maxTorque, ForceMode.Force);
            }
        }

        // --- Rewards ---

        // 1. Head height reward — normalized [0,1] toward target
        //float headHeight = headTransform.position.y;
        //float heightProgress = Mathf.Clamp01(headHeight / targetStandingHeight);
        //AddReward(heightProgress * heightRewardScale * Time.fixedDeltaTime);

        //// 2. Upright orientation reward — dot(hips.up, world.up)
        //float uprightDot = Vector3.Dot(hipsTransform.up, Vector3.up);
        //AddReward(Mathf.Clamp01(uprightDot) * 0.5f * Time.fixedDeltaTime);

        //// 3. Existential penalty
        //AddReward(existentialPenalty);

        //float distanceMoved = Vector3.Distance(hipsTransform.position, episodeStartPosition);
        //AddReward(Mathf.Clamp01(distanceMoved / 2f) * Time.fixedDeltaTime * 10f);

        //if (distanceMoved >= 10f) {
        //    AddReward(10f);
        //    EndEpisode();
        //}

        AddReward(hipsTransform.localPosition.z * -1 * Time.fixedDeltaTime);
       // AddReward(Mathf.Abs(hipsTransform.position.x) * -1 * Time.fixedDeltaTime);

        if(hipsTransform.localPosition.z < -10) {
            AddReward(5.0f);
            EndEpisode();
        }

        // 4. Standing success check
        //bool isUpright = uprightDot > 0.8f;
        //bool headAtHeight = headHeight >= targetStandingHeight - standingHeightThreshold;
        //if (isUpright && headAtHeight) {
        //    AddReward(standingBonus);
        //    EndEpisode();
        //}

    }

    [Header("Heuristic Testing")]
    public int testBodyPartIndex = 0; // change in Inspector at runtime

    public override void Heuristic(in ActionBuffers actionsOut) {
        //  testBodyPartIndex = (int) Mathf.Floor(Time.time) % bodyParts.Count;
        var continuous = actionsOut.ContinuousActions;
        for (int i = 0; i < continuous.Length; i++)
            continuous[i] = 0f;
        Debug.Log("Here");
        int b = testBodyPartIndex * 3;
        continuous[b + 0] = Input.GetAxis("Horizontal");  // X torque
        continuous[b + 1] = Input.GetKey(KeyCode.Q) ? 1f : Input.GetKey(KeyCode.E) ? -1f : 0f; // Y torque
        continuous[b + 2] = Input.GetAxis("Vertical");    // Z torque
    }


    #endregion

    // -----------------------------------------------------------------------
    #region Helpers

    /// <summary>
    /// Applies a velocity drive to the joint based on normalized action values.
    /// Uses AngularXDrive, AngularYZDrive and sets target velocity.
    /// </summary>
    private void ApplyJointTorque(Rigidbody rb, ConfigurableJoint joint, Vector3 normalizedAction) {
        // Build a drive with spring/damper set for responsiveness
        JointDrive drive = new JointDrive {
            positionSpring = 0f,
            positionDamper = maxTorque,
            maximumForce = maxTorque
        };

        joint.angularXDrive = drive;
        joint.angularYZDrive = drive;

        // Target angular velocity in joint local space
        joint.targetAngularVelocity = normalizedAction * maxTorque * 0.1f;
    }

    /// <summary>
    /// Resets all body parts to their initial pose with some random variation
    /// to teach robustness to different starting orientations (e.g. face-down, sideways).
    /// </summary>
    ///
    Vector3 episodeStartPosition;

    private void ResetRagdoll() {
        // Randomly spawn lying on back, front, or side
        Quaternion randomStartRotation = Quaternion.Euler(
            0.0f,
            Random.Range(0f, 0f),
            0.0f
        );

        // Reset hips / root first
        hipsTransform.position = initialHipsPosition + Vector3.up * spawnHeightOffset;
        hipsTransform.rotation = randomStartRotation;
        episodeStartPosition = hipsTransform.position;

        // Reset all body parts
        for (int i = 0; i < bodyParts.Count; i++) {
            var rb = bodyParts[i];

            // Reset kinematics so we can reposition
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            if (rb.transform != hipsTransform) {
                rb.transform.localPosition = initialPositions[i];
                rb.transform.localRotation = initialRotations[i];
            }

            // Reset joint targets
            if (joints[i] != null) {
                joints[i].targetAngularVelocity = Vector3.zero;
                joints[i].targetRotation = Quaternion.identity;
            }
        }
    }

    #endregion
}
