using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityStandardAssets.CrossPlatformInput;

/// <summary>
/// This class is for reinforcement learning on a AI Third Person Controller prefab
/// This requires an agent route for navigation, and
/// an current target Game Object for controlling the direction.
/// Be sure to place the agent on a NavMesh area.
/// </summary>
public class PedestrianFollowRL : Agent
{
    public AgentRoute agentRoute;
    private GameObject currentRouteDest; // Current route destination
    private int currentRouteIndex;

    private Vector3 defaultPosition;
    private const float RESET_BOUNDARY_LIMIT = 25f;
    private const float FOLLOW_MULTIPLIER = 1f;

    private int resetCounter = 0;
    private float previousSqrDistantToTarget;

    // Start is called before the first frame update
    void Start()
    {
        defaultPosition = this.transform.position;
        currentRouteDest = agentRoute.activeRoute[0].gameObject;
        currentRouteIndex = 0;

        previousSqrDistantToTarget = Vector3.SqrMagnitude(currentRouteDest.transform.position - this.transform.position);
    }

    public override void CollectObservations()
    {
        AddVectorObs((this.transform.position - defaultPosition).x / RESET_BOUNDARY_LIMIT);
        AddVectorObs((this.transform.position - defaultPosition).z / RESET_BOUNDARY_LIMIT);
        AddVectorObs((currentRouteDest.transform.position - defaultPosition).x / RESET_BOUNDARY_LIMIT);
        AddVectorObs((currentRouteDest.transform.position - defaultPosition).z / RESET_BOUNDARY_LIMIT);
    }

    public override void AgentReset()
    {
        currentRouteDest = agentRoute.activeRoute[0].gameObject;
        currentRouteIndex = 0;
        // Show current target
        agentRoute.activeRoute[0].gameObject.GetComponent<Renderer>().enabled = true;

        // Reset to original position
        this.transform.position = defaultPosition;
        this.GetComponent<AICharacterTarget>().ResetTarget();

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
        AddReward(-0.0001f);

        this.GetComponent<AICharacterTarget>().AddVector((vectorAction[0] * Vector3.forward + vectorAction[1] * Vector3.right) * FOLLOW_MULTIPLIER) ;

        // Reward: 
        if (Vector3.SqrMagnitude(this.transform.position - currentRouteDest.transform.position) < 2.25f)
        {
            // Hide target
            currentRouteDest.GetComponent<Renderer>().enabled = false;

            if (currentRouteIndex == agentRoute.activeRoute.Length - 1)
            {
                SetReward(1f);
                Done();
                Debug.Log("Reached final target");
            }
            else
            {
                currentRouteIndex++;
                currentRouteDest = agentRoute.activeRoute[currentRouteIndex].gameObject;
                // Show current target
                currentRouteDest.GetComponent<Renderer>().enabled = true;
                AddReward(0.1f);
                Debug.Log("Reached target #" + currentRouteIndex);
                this.GetComponent<AICharacterTarget>().ResetTarget();
            }
        }
        // Going too far. Reset!
        if (Vector3.SqrMagnitude(this.transform.position - currentRouteDest.transform.position) > RESET_BOUNDARY_LIMIT * RESET_BOUNDARY_LIMIT)
        {
            SetReward(-1f);
            Done();
        }
        // Encourage going closer to the object
        if (Vector3.SqrMagnitude(currentRouteDest.transform.position - this.transform.position) < previousSqrDistantToTarget)
        {
            AddReward(0.0003f);
            resetCounter = 0;
        }
        else
        {
            resetCounter++;
            // If going far from object for too long, need resetting target.
            if (resetCounter > 3) { this.GetComponent<AICharacterTarget>().ResetTarget(); resetCounter = 0; }
        }

        previousSqrDistantToTarget = Vector3.SqrMagnitude(currentRouteDest.transform.position - this.transform.position);
    }

}
