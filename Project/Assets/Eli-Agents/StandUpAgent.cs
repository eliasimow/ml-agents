using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.MLAgentsExamples;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class StandUpAgent : Agent {
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    JointDriveController m_JdController;

    [Header("Body Parts")]
    [Space(10)]
    public Transform pelvis;
    public Transform leftHip;
    public Transform leftKnee;
    public Transform leftFoot;

    public Transform rightHip;
    public Transform rightKnee;
    public Transform rightFoot;

    public Transform leftArm;
    public Transform leftElbow;
    public Transform rightArm;
    public Transform rightElbow;

    public Transform middleSpine;
    public Transform head;


    public override void Initialize() {
        m_JdController = GetComponent<JointDriveController>();

        //Setup each body part
        m_JdController.SetupBodyPart(pelvis);
        m_JdController.SetupBodyPart(leftHip);
        m_JdController.SetupBodyPart(leftKnee);
        m_JdController.SetupBodyPart(leftFoot);

        m_JdController.SetupBodyPart(rightHip);
        m_JdController.SetupBodyPart(rightKnee);
        m_JdController.SetupBodyPart(rightFoot);

        m_JdController.SetupBodyPart(leftArm);
        m_JdController.SetupBodyPart(leftElbow);

        m_JdController.SetupBodyPart(rightArm);
        m_JdController.SetupBodyPart(rightElbow);


        m_JdController.SetupBodyPart(middleSpine);
        m_JdController.SetupBodyPart(head);
    }

    public override void OnEpisodeBegin() {
        foreach (var bodyPart in m_JdController.bodyPartsDict.Values) {
            bodyPart.Reset(bodyPart);
        }

        //Random start rotation to help generalize
        pelvis.rotation = Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0);
    }


    public void CollectObservationBodyPart(BodyPart bp, VectorSensor sensor) {
        //GROUND CHECK
        sensor.AddObservation(bp.groundContact.touchingGround); // Is this bp touching the ground

        if (bp.rb.transform != pelvis) {
            sensor.AddObservation(bp.currentStrength / m_JdController.maxJointForceLimit);
        }
    }

    /// <summary>
    ///Returns the average velocity of all of the body parts
    ///Using the velocity of the body only has shown to result in more erratic movement from the limbs
    ///Using the average helps prevent this erratic movement
    /// </summary>
    Vector3 GetAvgVelocity() {
        Vector3 velSum = Vector3.zero;
        Vector3 avgVel = Vector3.zero;

        //ALL RBS
        int numOfRb = 0;
        foreach (var item in m_JdController.bodyPartsList) {
            numOfRb++;
            velSum += item.rb.linearVelocity;
        }

        avgVel = velSum / numOfRb;
        return avgVel;
    }

    public override void CollectObservations(VectorSensor sensor) {
        //ragdoll's avg vel
        var avgVel = GetAvgVelocity();

        //current ragdoll velocity. normalized
        sensor.AddObservation(avgVel);
        //rotation delta
        sensor.AddObservation(Quaternion.FromToRotation(pelvis.forward, Vector3.forward));


        RaycastHit hit;
        float maxRaycastDist = 10;
        if (Physics.Raycast(pelvis.position, Vector3.down, out hit, maxRaycastDist)) {
            sensor.AddObservation(hit.distance / maxRaycastDist);
        } else
            sensor.AddObservation(1);

        foreach (var bodyPart in m_JdController.bodyPartsList) {
            CollectObservationBodyPart(bodyPart, sensor);
        }
    }


    public override void OnActionReceived(ActionBuffers actionBuffers) {
        // The dictionary with all the body parts in it are in the jdController
        Debug.Log("received action");


        var bpDict = m_JdController.bodyPartsDict;

        var continuousActions = actionBuffers.ContinuousActions;
        var i = -1;
        // Pick a new target joint rotation
        bpDict[leftHip].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
        bpDict[leftKnee].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
        bpDict[leftFoot].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);

        bpDict[rightHip].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
        bpDict[rightKnee].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
        bpDict[rightFoot].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);

        bpDict[leftArm].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
        bpDict[leftElbow].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);

        bpDict[rightArm].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
        bpDict[rightElbow].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);

        bpDict[middleSpine].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);
        bpDict[head].SetJointTargetRotation(continuousActions[++i], continuousActions[++i], 0);


        // Update joint strength
        bpDict[leftHip].SetJointStrength(continuousActions[++i]);
        bpDict[leftKnee].SetJointStrength(continuousActions[++i]);
        bpDict[leftFoot].SetJointStrength(continuousActions[++i]);


        bpDict[rightHip].SetJointStrength(continuousActions[++i]);
        bpDict[rightKnee].SetJointStrength(continuousActions[++i]);
        bpDict[rightFoot].SetJointStrength(continuousActions[++i]);

        bpDict[leftArm].SetJointStrength(continuousActions[++i]);
        bpDict[leftElbow].SetJointStrength(continuousActions[++i]);

        bpDict[rightArm].SetJointStrength(continuousActions[++i]);
        bpDict[rightElbow].SetJointStrength(continuousActions[++i]);

        bpDict[middleSpine].SetJointStrength(continuousActions[++i]);
        bpDict[head].SetJointStrength(continuousActions[++i]);
    }

    float previousHeight = 0;
    void FixedUpdate() {
        float heightDelta = head.position.y - previousHeight;
        AddReward(heightDelta * 0.01f);
        previousHeight = head.position.y;
    }
}
