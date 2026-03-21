using System;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgentsExamples;
using Unity.MLAgents.Sensors;
using BodyPart = Unity.MLAgentsExamples.BodyPart;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEditor;
using UnityEngine.Profiling;

[System.Serializable]
public class StoredPose {
    public Quaternion hips;
    public Quaternion spine;
    public Quaternion head;

    public Quaternion thighL;
    public Quaternion shinL;
    public Quaternion footL;

    public Quaternion thighR;
    public Quaternion shinR;
    public Quaternion footR;

    public Quaternion armL;
    public Quaternion forearmL;
    public Quaternion handL;

    public Quaternion armR;
    public Quaternion forearmR;
    public Quaternion handR;
}
public class StanderThirdTry : Agent {
    [Header("T Pose")]
    public StoredPose tPose;

    [Header("Rest Pose")]
    public StoredPose restPose;

    [Header("Reference Pose")]
    public List<StoredPose> referencePoses;

    [ContextMenu("Set T Pose")]
    public void CaptureTPose() {
        tPose.hips = hips.localRotation;
        tPose.spine = spine.localRotation;
        tPose.head = head.localRotation;
        
        tPose.thighL = thighL.localRotation;
        tPose.shinL = shinL.localRotation;
        tPose.footL = footL.localRotation;
        
        tPose.thighR = thighR.localRotation;
        tPose.shinR = shinR.localRotation;
        tPose.footR = footR.localRotation;
        
        tPose.armL = armL.localRotation;
        tPose.forearmL = forearmL.localRotation;
        tPose.handL = handL.localRotation;
        
        tPose.armR = armR.localRotation;
        tPose.forearmR = forearmR.localRotation;
        tPose.handR = handR.localRotation;

        Debug.Log("Pose captured.");
    }

    [ContextMenu("Set Rest Pose")]
    public void CaptureRestPose() {
        restPose.hips = hips.localRotation;
        restPose.spine = spine.localRotation;
        restPose.head = head.localRotation;
        
        restPose.thighL = thighL.localRotation;
        restPose.shinL = shinL.localRotation;
        restPose.footL = footL.localRotation;
        
        restPose.thighR = thighR.localRotation;
        restPose.shinR = shinR.localRotation;
        restPose.footR = footR.localRotation;
        
        restPose.armL = armL.localRotation;
        restPose.forearmL = forearmL.localRotation;
        restPose.handL = handL.localRotation;
        
        restPose.armR = armR.localRotation;
        restPose.forearmR = forearmR.localRotation;
        restPose.handR = handR.localRotation;

        Debug.Log("Pose captured.");
    }

    [ContextMenu("Return to Rest Pose")]
    public void ToRestPose() {
        hips.localRotation = restPose.hips;
        spine.localRotation = restPose.spine;
        head.localRotation = restPose.head;

        thighL.localRotation = restPose.thighL;
        shinL.localRotation = restPose.shinL;
        footL.localRotation = restPose.footL;

        thighR.localRotation = restPose.thighR;
        shinR.localRotation = restPose.shinR;
        footR.localRotation = restPose.footR;

        armL.localRotation = restPose.armL;
        forearmL.localRotation = restPose.forearmL;
        handL.localRotation = restPose.handL;

        armR.localRotation = restPose.armR;
        forearmR.localRotation = restPose.forearmR;
        handR.localRotation = restPose.handR;

        Debug.Log("Pose reset to stored T-pose.");
    }

    [ContextMenu("Return to T Pose")]
    public void ResetPose() {
        hips.localRotation = tPose.hips;
        spine.localRotation = tPose.spine;
        head.localRotation = tPose.head;

        thighL.localRotation = tPose.thighL;
        shinL.localRotation = tPose.shinL;
        footL.localRotation = tPose.footL;

        thighR.localRotation = tPose.thighR;
        shinR.localRotation = tPose.shinR;
        footR.localRotation = tPose.footR;

        armL.localRotation = tPose.armL;
        forearmL.localRotation = tPose.forearmL;
        handL.localRotation = tPose.handL;

        armR.localRotation = tPose.armR;
        forearmR.localRotation = tPose.forearmR;
        handR.localRotation = tPose.handR;

        Debug.Log("Pose reset to stored T-pose.");
    }

    [ContextMenu("Capture Current Pose")]
    public void CaptureCurrentPose() {
        StoredPose referencePose = new StoredPose();

        referencePose.hips = hips.localRotation;
        referencePose.spine = spine.localRotation;
        referencePose.head = head.localRotation;

        referencePose.thighL = thighL.localRotation;
        referencePose.shinL = shinL.localRotation;
        referencePose.footL = footL.localRotation;

        referencePose.thighR = thighR.localRotation;
        referencePose.shinR = shinR.localRotation;
        referencePose.footR = footR.localRotation;

        referencePose.armL = armL.localRotation;
        referencePose.forearmL = forearmL.localRotation;
        referencePose.handL = handL.localRotation;

        referencePose.armR = armR.localRotation;
        referencePose.forearmR = forearmR.localRotation;
        referencePose.handR = handR.localRotation;

        referencePoses.Add(referencePose);
        Debug.Log("Pose captured.");
    }

    [ContextMenu("Return to Reference Pose")]
    public void ToReferencePose() {
        StoredPose referencePose = referencePoses[0];
        hips.localRotation = referencePose.hips;
        spine.localRotation = referencePose.spine;
        head.localRotation = referencePose.head;

        thighL.localRotation = referencePose.thighL;
        shinL.localRotation = referencePose.shinL;
        footL.localRotation = referencePose.footL;

        thighR.localRotation = referencePose.thighR;
        shinR.localRotation = referencePose.shinR;
        footR.localRotation = referencePose.footR;

        armL.localRotation = referencePose.armL;
        forearmL.localRotation = referencePose.forearmL;
        handL.localRotation = referencePose.handL;

        armR.localRotation = referencePose.armR;
        forearmR.localRotation = referencePose.forearmR;
        handR.localRotation = referencePose.handR;

        Debug.Log("Pose reset to stored T-pose.");
    }

    [Header("Target To Walk Towards")] public Transform target; //Target the agent will walk towards during training.

    [Header("Walk Speed")]
    [Range(0.1f, 10)]
    [SerializeField]
    //The walking speed to try and achieve
    private float m_TargetWalkingSpeed = 10;

    public float MTargetWalkingSpeed // property
    {
        get { return m_TargetWalkingSpeed; }
        set { m_TargetWalkingSpeed = Mathf.Clamp(value, .1f, m_maxWalkingSpeed); }
    }

    const float m_maxWalkingSpeed = 10; //The max walking speed

    //Should the agent sample a new goal velocity each episode?
    //If true, walkSpeed will be randomly set between zero and m_maxWalkingSpeed in OnEpisodeBegin()
    //If false, the goal velocity will be walkingSpeed
    public bool randomizeWalkSpeedEachEpisode;

    //The direction an agent will walk during training.
    private Vector3 m_WorldDirToWalk = Vector3.right;


    [Header("Body Parts")] public Transform hips;
    public Transform spine;
    public Transform head;
    public Transform thighL;
    public Transform shinL;
    public Transform footL;
    public Transform thighR;
    public Transform shinR;
    public Transform footR;
    public Transform armL;
    public Transform forearmL;
    public Transform handL;
    public Transform armR;
    public Transform forearmR;
    public Transform handR;

    //This will be used as a stabilized model space reference point for observations
    //Because ragdolls can move erratically during training, using a stabilized reference transform improves learning
    OrientationCubeController m_OrientationCube;

    //The indicator graphic gameobject that points towards the target
    DirectionIndicator m_DirectionIndicator;
    JointDriveController m_JdController;
    EnvironmentParameters m_ResetParams;

    float timeInLoop = 0.0f;

    public override void Initialize() {
        m_OrientationCube = GetComponentInChildren<OrientationCubeController>();
        m_DirectionIndicator = GetComponentInChildren<DirectionIndicator>();

        //Setup each body part
        m_JdController = GetComponent<JointDriveController>();
        m_JdController.SetupBodyPart(hips);
        m_JdController.SetupBodyPart(spine);
        m_JdController.SetupBodyPart(head);
        m_JdController.SetupBodyPart(thighL);
        m_JdController.SetupBodyPart(shinL);
        m_JdController.SetupBodyPart(footL);
        m_JdController.SetupBodyPart(thighR);
        m_JdController.SetupBodyPart(shinR);
        m_JdController.SetupBodyPart(footR);
        m_JdController.SetupBodyPart(armL);
        m_JdController.SetupBodyPart(forearmL);
        m_JdController.SetupBodyPart(handL);
        m_JdController.SetupBodyPart(armR);
        m_JdController.SetupBodyPart(forearmR);
        m_JdController.SetupBodyPart(handR);

        m_ResetParams = Academy.Instance.EnvironmentParameters;
    }

    public override void OnEpisodeBegin() {
        //Reset all of the body parts
        foreach (var bodyPart in m_JdController.bodyPartsDict.Values) {
            bodyPart.Reset(bodyPart);
        }

        ToRestPose();
        
        //Random start rotation to help generalize
   //     hips.rotation = Quaternion.Euler(hips.rotation.x, Random.Range(0.0f, 360.0f), hips.rotation.z);
        Debug.Log("randomized");

        UpdateOrientationObjects();

        timeInLoop = 0.0f;

        //Set our goal walking speed
        MTargetWalkingSpeed =
            randomizeWalkSpeedEachEpisode ? Random.Range(0.1f, m_maxWalkingSpeed) : MTargetWalkingSpeed;
    }

    /// <summary>
    /// Add relevant information on each body part to observations.
    /// </summary>
    public void CollectObservationBodyPart(BodyPart bp, VectorSensor sensor) {
        //GROUND CHECK
        sensor.AddObservation(bp.groundContact.touchingGround); // Is this bp touching the ground

        //Get velocities in the context of our orientation cube's space
        //Note: You can get these velocities in world space as well but it may not train as well.
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(bp.rb.linearVelocity));
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(bp.rb.angularVelocity));

        //Get position relative to hips in the context of our orientation cube's space
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(bp.rb.position - hips.position));

        if (bp.rb.transform != hips && bp.rb.transform != handL && bp.rb.transform != handR) {
            sensor.AddObservation(bp.rb.transform.localRotation);
            sensor.AddObservation(bp.currentStrength / m_JdController.maxJointForceLimit);
        }
    }

    /// <summary>
    /// Loop over body parts to add them to observation.
    /// </summary>
    public override void CollectObservations(VectorSensor sensor) {
        var cubeForward = m_OrientationCube.transform.forward;

        //velocity we want to match
        var velGoal = cubeForward * MTargetWalkingSpeed;
        //ragdoll's avg vel
        var avgVel = GetAvgVelocity();

        //current ragdoll velocity. normalized
        sensor.AddObservation(Vector3.Distance(velGoal, avgVel));
        //avg body vel relative to cube
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(avgVel));
        //vel goal relative to cube
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformDirection(velGoal));

        //rotation deltas
        sensor.AddObservation(Quaternion.FromToRotation(hips.forward, cubeForward));
        sensor.AddObservation(Quaternion.FromToRotation(head.forward, cubeForward));

        //Position of target position relative to cube
        sensor.AddObservation(m_OrientationCube.transform.InverseTransformPoint(target.transform.position));

        foreach (var bodyPart in m_JdController.bodyPartsList) {
            CollectObservationBodyPart(bodyPart, sensor);
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers) {
        var bpDict = m_JdController.bodyPartsDict;
        var i = -1;

        var continuousActions = actionBuffers.ContinuousActions;
        bpDict[spine].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], continuousActions[++i]);

        bpDict[thighL].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
        bpDict[thighR].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
        bpDict[shinL].SetJointTargetRotation(continuousActions[++i], 0, 0);
        bpDict[shinR].SetJointTargetRotation(continuousActions[++i], 0, 0);
        bpDict[footR].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], continuousActions[++i]);
        bpDict[footL].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], continuousActions[++i]);

        bpDict[armL].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
        bpDict[armR].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
        bpDict[forearmL].SetJointTargetRotation(continuousActions[++i], 0, 0);
        bpDict[forearmR].SetJointTargetRotation(continuousActions[++i], 0, 0);
        bpDict[head].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);

        //update joint strength settings
        bpDict[spine].SetJointStrength(continuousActions[++i]);
        bpDict[head].SetJointStrength(continuousActions[++i]);
        bpDict[thighL].SetJointStrength(continuousActions[++i]);
        bpDict[shinL].SetJointStrength(continuousActions[++i]);
        bpDict[footL].SetJointStrength(continuousActions[++i]);
        bpDict[thighR].SetJointStrength(continuousActions[++i]);
        bpDict[shinR].SetJointStrength(continuousActions[++i]);
        bpDict[footR].SetJointStrength(continuousActions[++i]);
        bpDict[armL].SetJointStrength(continuousActions[++i]);
        bpDict[forearmL].SetJointStrength(continuousActions[++i]);
        bpDict[armR].SetJointStrength(continuousActions[++i]);
        bpDict[forearmR].SetJointStrength(continuousActions[++i]);
    }

    //Update OrientationCube and DirectionIndicator
    void UpdateOrientationObjects() {
        m_WorldDirToWalk = target.position - hips.position;
        m_OrientationCube.UpdateOrientation(hips, target);
        if (m_DirectionIndicator) {
            m_DirectionIndicator.MatchOrientation(m_OrientationCube.transform);
        }
    }
    void FixedUpdate() {
        //    Vector3 input = new Vector3(
        //    Input.GetAxis("Horizontal"),
        //    0,
        //    Input.GetAxis("Vertical")
        //);

        //foreach (var bodyPart in m_JdController.bodyPartsList) {
        //    if (!bodyPart.Equals(hips)) {
        //        float h = Input.GetAxis("Horizontal"); // A/D
        //        float v = Input.GetAxis("Vertical");   // W/S

        //        // Update rotation
        //        currentEuler.x += v * rotationSpeed * Time.fixedDeltaTime; // pitch
        //        currentEuler.y += h * rotationSpeed * Time.fixedDeltaTime; // yaw


        //        bodyPart.SetJointTargetRotation(currentEuler.x, currentEuler.y, currentEuler.z);
        //        bodyPart.SetJointStrength(1.0f);             
        //    }
        //}


        UpdateOrientationObjects();

        timeInLoop += Time.fixedDeltaTime;

        AddReward(GetPoseTrackingReward(Time.fixedDeltaTime)/10.0f);
        CheckPoseCompletion();
        return;

        /*
        var cubeForward = m_OrientationCube.transform.forward;

        // Set reward for this step according to mixture of the following elements.
        // a. Match target speed
        //This reward will approach 1 if it matches perfectly and approach zero as it deviates
        var matchSpeedReward = GetMatchingVelocityReward(cubeForward * MTargetWalkingSpeed, GetAvgVelocity());

        //Check for NaNs
        if (float.IsNaN(matchSpeedReward)) {
            throw new ArgumentException(
                "NaN in moveTowardsTargetReward.\n" +
                $" cubeForward: {cubeForward}\n" +
                $" hips.velocity: {m_JdController.bodyPartsDict[hips].rb.linearVelocity}\n" +
                $" maximumWalkingSpeed: {m_maxWalkingSpeed}"
            );
        }

        // b. Rotation alignment with target direction.
        //This reward will approach 1 if it faces the target direction perfectly and approach zero as it deviates
        var headForward = head.forward;
        headForward.y = 0;
        // var lookAtTargetReward = (Vector3.Dot(cubeForward, head.forward) + 1) * .5F;
        var lookAtTargetReward = (Vector3.Dot(cubeForward, headForward) + 1) * .5F;

        //Check for NaNs
        if (float.IsNaN(lookAtTargetReward)) {
            throw new ArgumentException(
                "NaN in lookAtTargetReward.\n" +
                $" cubeForward: {cubeForward}\n" +
                $" head.forward: {head.forward}"
            );
        }

        AddReward(matchSpeedReward * lookAtTargetReward);
        */
    }

    //Returns the average velocity of all of the body parts
    //Using the velocity of the hips only has shown to result in more erratic movement from the limbs, so...
    //...using the average helps prevent this erratic movement
    Vector3 GetAvgVelocity() {
        Vector3 velSum = Vector3.zero;

        //ALL RBS
        int numOfRb = 0;
        foreach (var item in m_JdController.bodyPartsList) {
            numOfRb++;
            velSum += item.rb.linearVelocity;
        }

        var avgVel = velSum / numOfRb;
        return avgVel;
    }

    //normalized value of the difference in avg speed vs goal walking speed.
    public float GetMatchingVelocityReward(Vector3 velocityGoal, Vector3 actualVelocity) {
        //distance between our actual velocity and goal velocity
        var velDeltaMagnitude = Mathf.Clamp(Vector3.Distance(actualVelocity, velocityGoal), 0, MTargetWalkingSpeed);

        //return the value on a declining sigmoid shaped curve that decays from 1 to 0
        //This reward will approach 1 if it matches perfectly and approach zero as it deviates
        return Mathf.Pow(1 - Mathf.Pow(velDeltaMagnitude / MTargetWalkingSpeed, 2), 2);
    }

    /// <summary>
    /// Agent touched the target
    /// </summary>
    public void TouchedTarget() {
        AddReward(1f);
    }

    float GetPoseTrackingReward(float deltaTime) {
        float best = 0f;

        foreach (var pose in referencePoses) {
            float sim = PoseSimilarity(pose); // your previous function (0–1)
            if (sim > best) best = sim;
        }

        // Shape it so it strongly prefers high similarity
        float shaped = best * best;

        // Scale by time so reward/sec is consistent
        return shaped * deltaTime;
    }

    void CheckPoseCompletion() {
        float best = 0f;

        foreach (var pose in referencePoses) {
            float sim = PoseSimilarity(pose);
            if (sim > best) best = sim;
        }

        const float SUCCESS_THRESHOLD = 0.98f;
        const float MAX_TIME = 12.0f;

        if (best >= SUCCESS_THRESHOLD) {
            float t = Mathf.Clamp01(timeInLoop / MAX_TIME);

            float timeMultiplier = Mathf.Pow(1.0f - t, 2f);

            float reward = 10.0f * timeMultiplier;

            AddReward(reward);

            Debug.Log($"Done! {best:F3} | t={timeInLoop:F2}s | reward={reward:F2}");
            EndEpisode();
        }
    }
    float QuaternionSimilarity(Quaternion a, Quaternion b) {
        float similarity = Mathf.Abs(Quaternion.Dot(a, b)); // 1 = identical
        return similarity;
    }
    Quaternion RemoveYaw(Quaternion q) {
        Vector3 forward = q * Vector3.forward;
        forward.y = 0f;

        if (forward.sqrMagnitude < 1e-6f)
            return Quaternion.identity;

        return Quaternion.LookRotation(forward.normalized, Vector3.up);
    }

    float PoseSimilarity(StoredPose refPose) {
        float total = 0f;
        float weightSum = 0f;

        void Add(float sim, float w) {
            total += sim * w;
            weightSum += w;
        }

        // --- HIPS (ignore Y rotation) ---
        Quaternion hipsCurrent = RemoveYaw(hips.localRotation);
        Quaternion hipsRef = RemoveYaw(refPose.hips);
        Add(QuaternionSimilarity(hipsCurrent, hipsRef), 4.0f); // MOST IMPORTANT

        // --- CORE ---
        Add(QuaternionSimilarity(spine.localRotation, refPose.spine), 3.0f);
        Add(QuaternionSimilarity(head.localRotation, refPose.head), 1.0f);

        // --- LEGS (very important for standing) ---
        Add(QuaternionSimilarity(thighL.localRotation, refPose.thighL), 2.5f);
        Add(QuaternionSimilarity(shinL.localRotation, refPose.shinL), 2.0f);
        Add(QuaternionSimilarity(footL.localRotation, refPose.footL), 1.5f);

        Add(QuaternionSimilarity(thighR.localRotation, refPose.thighR), 2.5f);
        Add(QuaternionSimilarity(shinR.localRotation, refPose.shinR), 2.0f);
        Add(QuaternionSimilarity(footR.localRotation, refPose.footR), 1.5f);

        // --- ARMS (useful for pushing, but less critical) ---
        Add(QuaternionSimilarity(armL.localRotation, refPose.armL), 1.2f);
        Add(QuaternionSimilarity(forearmL.localRotation, refPose.forearmL), 1.0f);
        Add(QuaternionSimilarity(handL.localRotation, refPose.handL), 0.5f);

        Add(QuaternionSimilarity(armR.localRotation, refPose.armR), 1.2f);
        Add(QuaternionSimilarity(forearmR.localRotation, refPose.forearmR), 1.0f);
        Add(QuaternionSimilarity(handR.localRotation, refPose.handR), 0.5f);

        return total / weightSum; // still 0–1
    }
}
