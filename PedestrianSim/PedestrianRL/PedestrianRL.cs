using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.CrossPlatformInput;

public class PedestrianRL : Agent
{
    public AgentRoute agentRoute;
    private GameObject currentTarget;
    private int currentTargetIndex;

    private Vector3 defaultPosition;
    private const float BOUNDARY_LIMIT = 100f;
    private const float SQR_RESET_BOUNDARY_LIMIT = 225f;

    //// - THIRD-PERSON CONTROLLER CODES - 
    private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
    private Transform m_Cam;                  // A reference to the main camera in the scenes transform
    private Vector3 m_CamForward;             // The current forward direction of the camera
    private Vector3 m_Move;
    private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
    //// - END THIRD-PERSON CONTROLLER CODES -

    private float previousSqrDistant;
    private Vector3 previousM_Move;
    private float prevH, prevV;
    // Start is called before the first frame update
    void Start()
    {
        defaultPosition = this.transform.position;
        currentTarget = agentRoute.activeRoute[0].gameObject;
        currentTargetIndex = 0;

        previousSqrDistant = Vector3.SqrMagnitude(currentTarget.transform.position - this.transform.position);

        //// - THIRD-PERSON CONTROLLER CODES - 
        // get the transform of the main camera
        if (Camera.main != null)
        {
            m_Cam = Camera.main.transform;
        }
        else
        {
            Debug.LogWarning(
                "Warning: no main camera found. Third person character needs a Camera tagged \"MainCamera\", for camera-relative controls.", gameObject);
            // we use self-relative controls in this case, which probably isn't what the user wants, but hey, we warned them!
        }

        // get the third person character ( this should never be null due to require component )
        m_Character = GetComponent<ThirdPersonCharacter>();
        //// - END THIRD-PERSON CONTROLLER CODES -

    }

    public override void CollectObservations()
    {

        AddVectorObs((this.transform.position - defaultPosition).x / 50f);
        AddVectorObs((this.transform.position - defaultPosition).z / 50f);
        AddVectorObs((currentTarget.transform.position - defaultPosition).x / 50f);
        AddVectorObs((currentTarget.transform.position - defaultPosition).z / 50f);

        //Debug.Log((this.transform.position - defaultPosition).x / 50f + " - " +
        //          (this.transform.position - defaultPosition).z / 50f + " - " +
        //          (currentTarget.transform.position - defaultPosition).x / 50f + " - " +
        //          (currentTarget.transform.position - defaultPosition).z / 50f);
        //// Observe the closest agent's direction and vector
        //GameObject pedestrian = GetClosestPedestrian();
        //AddVectorObs(pedestrian.transform.position);
        //AddVectorObs(pedestrian.transform.rotation.eulerAngles.y);
    }

    public override void AgentReset()
    {
        currentTarget = agentRoute.activeRoute[0].gameObject;
        currentTargetIndex = 0;

        // Reset to original position
        this.transform.position = defaultPosition;
        // Randomise route;
        //currentTarget.transform.position = new Vector3(Random.Range(-7f, 7f), 1f, Random.Range(-7f, 7f));
        agentRoute.activeRoute[0].gameObject.transform.position = defaultPosition + new Vector3(Random.Range(-7f, 7f), 0.5f, Random.Range(-7f, 7f));
        for (int i = 1; i < agentRoute.activeRoute.Length; i++)
        {
            agentRoute.activeRoute[i].gameObject.transform.position =
                agentRoute.activeRoute[i - 1].gameObject.transform.position + new Vector3(Random.Range(-7f, 7f), 0, Random.Range(-7f, 7f));
        }
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Encourage the agent go to the destination in the shortest time
        AddReward(-0.0003f);

        //float h = Mathf.Clamp(vectorAction[0], -1.0f, 1.0f);
        //float v = Mathf.Clamp(vectorAction[1], -1.0f, 1.0f);

        //this.transform.position += new Vector3(h, 0, v) * 0.03f;

        ////// - THIRD-PERSON CONTROLLER CODES - 
        // read inputs

        ////! EIGHT MOVEMENT DIRECTION
        //float h, v;
        //if (vectorAction[0] > 0) h = -1f; else h = 1f;
        //if (vectorAction[1] > 0) v = -1f; else v = 1f;

        m_Move = (vectorAction[0] * Vector3.forward + vectorAction[1] * Vector3.right).normalized;
        m_Move *= 0.4f;
        //Debug.Log(m_Move);
        ////! ANGULAR MOVEMENT
        //float h, v;
        //if (Mathf.Abs(vectorAction[0]) < 0.001f) h = 0; else h = vectorAction[0];
        //if (vectorAction[1] > 0) v = vectorAction[1] / 3; else v = 0f;
        //m_Move = (v * this.transform.forward + h * this.transform.right).normalized;
        //m_Move *= 0.4f;

        // pass all parameters to the character control script
        m_Character.Move(m_Move, false, m_Jump);
        //// - END THIRD-PERSON CONTROLLER CODES -

        // Reward: 
        //// Reach single target
        //if (Vector3.SqrMagnitude(this.transform.position - currentTarget.transform.position) < 2f)
        //{
        //    SetReward(1f); 
        //    Done();
        //    Debug.Log("Something");
        //}
        // Reach target
        if (Vector3.SqrMagnitude(this.transform.position - currentTarget.transform.position) < 2.25f)
        {
            if (currentTargetIndex == agentRoute.activeRoute.Length - 1)
            {
                SetReward(1f);
                Done();
                Debug.Log("Reached final target");
            }
            else
            {
                currentTargetIndex++;
                currentTarget = agentRoute.activeRoute[currentTargetIndex].gameObject;
                AddReward(0.1f);
                Debug.Log("Reached target #" + currentTargetIndex);
            }
        }
        // Going too far. Reset!
        if (Vector3.SqrMagnitude(this.transform.position - currentTarget.transform.position) > SQR_RESET_BOUNDARY_LIMIT)
        {
            SetReward(-1f);
            Done();
        }
        // Encourage going closer to the object
        if (Vector3.SqrMagnitude(currentTarget.transform.position - this.transform.position) < previousSqrDistant)
        {
            AddReward(0.0001f);
        }
        // Discourage changing direction
        //AddReward(-0.00003f * Vector3.Angle(m_Move, previousM_Move));
        //if (h != prevH || v != prevV) // For eight-way movement
        //    AddReward(-0.00003f);
        //AddReward(-0.0003f * Mathf.Abs(h));  // For angular movement
        //if (h == 0) AddReward(0.0001f);
        //else AddReward(-0.0001f);

        //prevH = h; prevV = v;

        previousM_Move = m_Move;
        previousSqrDistant = Vector3.SqrMagnitude(currentTarget.transform.position - this.transform.position);
    }

    public GameObject GetClosestPedestrian()
    {
        Collider[] colliders = Physics.OverlapSphere(this.transform.position, BOUNDARY_LIMIT);
        Collider closestCollider = null;
        
        foreach (Collider hit in colliders)
        {
            if (!hit.gameObject.CompareTag("Pedestrian"))
            {
                continue;
            }
            if (!closestCollider)
            {
                closestCollider = hit;
            }
            if (Vector3.Distance(transform.position, hit.transform.position) <= Vector3.Distance(transform.position, closestCollider.transform.position))
            {
                closestCollider = hit;
            }
        }
        return closestCollider.gameObject;

    }
}
