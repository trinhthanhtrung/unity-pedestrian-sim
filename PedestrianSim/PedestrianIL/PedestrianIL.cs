using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.CrossPlatformInput;

/// <summary>
/// This class is for imitation learning on a Third Person Controller prefab
/// Most of these codes are copied from PedestrianRL, however most reward-related
/// instructions are not neccessary.
/// </summary>
/// <seealso cref="PedestrianRL"/>
public class PedestrianIL : Agent
{
    public AgentRoute agentRoute;
    private GameObject currentTarget;
    private int currentTargetIndex;

    private Vector3 defaultPosition;
    private const float BOUNDARY_LIMIT = 25f;
    private const float SQR_RESET_BOUNDARY_LIMIT = 225f;
               
    //// - THIRD-PERSON CONTROLLER CODES - 
    private ThirdPersonCharacter m_Character; // A reference to the ThirdPersonCharacter on the object
    private Transform m_Cam;                  // A reference to the main camera in the scenes transform
    private Vector3 m_CamForward;             // The current forward direction of the camera
    private Vector3 m_Move;
    private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.
    //// - END THIRD-PERSON CONTROLLER CODES -

    private float previousSqrDistant;
    private GameObject closestPedestrian;
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

        //// Observe the closest agent's direction and vector
        closestPedestrian = GetClosestPedestrian();
        if (closestPedestrian != null)
        {
            AddVectorObs((closestPedestrian.transform.position - defaultPosition).x / 50f);
            AddVectorObs((closestPedestrian.transform.position - defaultPosition).z / 50f);
            AddVectorObs(closestPedestrian.transform.rotation.eulerAngles.y / 360f);
        }
        else
        {
            AddVectorObs(-1f);
            AddVectorObs(-1f);
            AddVectorObs(-1f);
        }

    }

    public override void AgentReset()
    {
        currentTarget = agentRoute.activeRoute[0].gameObject;
        currentTargetIndex = 0;
        // Show current target
        currentTarget.GetComponent<Renderer>().enabled = true;

        // Reset to original position
        this.transform.position = defaultPosition;
        // Randomise route
        agentRoute.activeRoute[0].gameObject.transform.position = defaultPosition + new Vector3(Random.Range(-3f, 3f), 0.5f, Random.Range(-3f, 3f));
        for (int i = 1; i < agentRoute.activeRoute.Length; i++)
        {
            agentRoute.activeRoute[i].gameObject.transform.position =
                agentRoute.activeRoute[i - 1].gameObject.transform.position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
        }
        //// Slightly randomise each node
        //for (int i = 0; i < agentRoute.activeRoute.Length; i++)
        //{
        //    agentRoute.activeRoute[i].gameObject.transform.position += new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
        //}

    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        // Encourage the agent go to the destination in the shortest time
        AddReward(-0.0003f);

        ////// - THIRD-PERSON CONTROLLER CODES - 
        // read inputs
        m_Move = (vectorAction[0] * Vector3.forward + vectorAction[1] * Vector3.right).normalized;
        m_Move *= 0.4f;
        // pass all parameters to the character control script
        m_Character.Move(m_Move, false, m_Jump);
        //// - END THIRD-PERSON CONTROLLER CODES -

        // Reward: 
        if (Vector3.SqrMagnitude(this.transform.position - currentTarget.transform.position) < 2.25f)
        {
            // Hide previous target
            agentRoute.activeRoute[currentTargetIndex].gameObject.GetComponent<Renderer>().enabled = false;

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
                // Show current target
                currentTarget.GetComponent<Renderer>().enabled = true;
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
        previousSqrDistant = Vector3.SqrMagnitude(currentTarget.transform.position - this.transform.position);

        // Too close to another pedestrian!
        if (closestPedestrian != null)
            if (Vector3.SqrMagnitude(this.transform.position - closestPedestrian.transform.position) < 1f)
            {
                AddReward(-0.001f);
                Debug.Log("Bumped into someone.");
            }


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
        if (!closestCollider)
            return null;
        else
            return closestCollider.gameObject;
    }
}
