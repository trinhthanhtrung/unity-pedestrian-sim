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
public class PedestrianAvoidRL : Agent
{
    public Transform currentDirection; // Current direction target

    private Vector3 defaultPosition;
    private const float BOUNDARY_LIMIT = 25f;

    private const float AVOID_MULTIPLIER = 1f;

    private float previousSqrDistantToTarget;
    private GameObject closestPedestrian;
    private GameObject previousClosestPedestrian;
    private float previousSqrDistantToPedestrian;

    // Start is called before the first frame update
    void Start()
    {
        defaultPosition = this.transform.position;
        previousSqrDistantToPedestrian = 100f;
    }

    public override void CollectObservations()
    {
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
        // Reset to original position
        this.transform.position = defaultPosition;
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        //currentDirection.position += (vectorAction[0] * Vector3.forward + vectorAction[1] * Vector3.right) * AVOID_MULTIPLIER;
        this.GetComponent<AICharacterTarget>().AddVector((vectorAction[0] * Vector3.forward + vectorAction[1] * Vector3.right) * AVOID_MULTIPLIER);

        //Debug.Log(vectorAction[0] + ";" + vectorAction[1]);
        // Reward: 
        // Too close to another pedestrian!
        if (closestPedestrian != null)
        {
            float sqrDistantToPedestrian = Vector3.SqrMagnitude(this.transform.position - closestPedestrian.transform.position);
            // Encourage going further from other pedestrian (ONLY when close enough)

            if (sqrDistantToPedestrian < BOUNDARY_LIMIT && closestPedestrian == previousClosestPedestrian) // Still the same pedestrian
            {   // Encourage going further away the closer the pedestrian is
                if (previousSqrDistantToPedestrian < sqrDistantToPedestrian)
                    AddReward(0.00003f * (BOUNDARY_LIMIT - sqrDistantToPedestrian));
            }
            if (sqrDistantToPedestrian < 1f)
            {
                AddReward(-0.005f);
                //SetReward(-1f); Done();
                Debug.Log("Bumped into someone.");
            }
            previousSqrDistantToPedestrian = sqrDistantToPedestrian;
            previousClosestPedestrian = closestPedestrian;
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
