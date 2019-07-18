using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class SFCharacter : MonoBehaviour
{
    Vector3 velocity = new Vector3();
    public Vector3 Velocity
    {
        get { return velocity; }
    }

    AICharacterControl characterControl;
    NavMeshAgent characterAgent;
    float radius;

    public Wall[] walls;
    public SFCharacter[] agents;
    public Transform destination;
    public float desiredSpeed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        characterControl = this.GetComponent<AICharacterControl>();
        characterAgent = this.GetComponent<NavMeshAgent>();
        if (this.destination == null) this.destination = this.transform;
        else 
            if (characterControl)
                characterControl.target.localPosition = velocity;
        if (characterAgent)
            characterAgent.speed = desiredSpeed;
        radius = 0.5f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 acceleration;

        acceleration = DrivingForce() + AgentInteractForce() + WallInteractForce();
        velocity += acceleration * Time.deltaTime * 3;


        // Limit maximum velocity
        if (Vector3.SqrMagnitude(velocity) > desiredSpeed * desiredSpeed)
        {
            velocity.Normalize();
            velocity *= desiredSpeed;
        }

        // Update current attributes to AICharacterControl
        if (characterControl)
            characterControl.target.transform.position = this.transform.position + velocity.normalized;
        else
           this.transform.position += velocity * Time.deltaTime * 5;

        if (characterAgent)
            characterAgent.speed = desiredSpeed;
    }

    Vector3 DrivingForce()
    {
        const float relaxationT = 0.54f;
        Vector3 desiredDirection = destination.transform.position - this.transform.position;
        desiredDirection.Normalize();

        Vector3 drivingForce = (desiredSpeed * desiredDirection - velocity) / relaxationT;

        return drivingForce;
    }

    Vector3 AgentInteractForce()
    {
        const float lambda = 2.0f;
        const float gamma = 0.35f;
        const float nPrime = 3.0f;
        const float n = 2.0f;
        //const float A = 4.5f;
        const float A = 47f;
        float B, theta;
        int K;
        Vector3 interactionForce = new Vector3(0f, 0f, 0f);

        Vector3 vectorToAgent = new Vector3();

        foreach (SFCharacter agent in agents)
        {
            // Skip if agent is self
            if (agent == this) continue;

            vectorToAgent = agent.transform.position - this.transform.position;

            // Skip if agent is too far
            if (Vector3.SqrMagnitude(vectorToAgent) > 10f * 10f) continue;

            Vector3 directionToAgent = vectorToAgent.normalized;
            Vector3 interactionVector = lambda * (this.Velocity - agent.Velocity) + directionToAgent;

            B = gamma * Vector3.Magnitude(interactionVector);

            Vector3 interactionDir = interactionVector.normalized;

            theta = Mathf.Deg2Rad * Vector3.Angle(interactionDir, directionToAgent);

            if (theta == 0) K = 0;
            else if (theta > 0) K = 1;
            else K = -1;

            float distanceToAgent = Vector3.Magnitude(vectorToAgent);

            float deceleration = -A * Mathf.Exp(-distanceToAgent / B
                                                - (nPrime * B * theta) * (nPrime * B * theta));
            float directionalChange = -A * K * Mathf.Exp(-distanceToAgent / B
                                                         - (n * B * theta) * (n * B * theta));
            Vector3 normalInteractionVector = new Vector3(-interactionDir.z, interactionDir.y, interactionDir.x);
            //Vector3 normalInteractionVector = new Vector3(-interactionDir.y, interactionDir.x, 0);

            interactionForce += deceleration * interactionDir + directionalChange * normalInteractionVector; 
        }
        return interactionForce;
    }

    Vector3 WallInteractForce()
    {
        const float A = 3f;
        const float B = 0.8f;

        float squaredDist = Mathf.Infinity;
        float minSquaredDist = Mathf.Infinity;
        Vector3 minDistVector = new Vector3();

        // Find distance to nearest obstacles
        foreach (Wall w in walls)
        {
            Vector3 vectorToNearestPoint = this.transform.position - w.GetNearestPoint(this.transform.position);
            squaredDist = Vector3.SqrMagnitude(vectorToNearestPoint);

            if (squaredDist < minSquaredDist)
            {
                minSquaredDist = squaredDist;
                minDistVector = vectorToNearestPoint;
            }
        }

        float distToNearestObs = Mathf.Sqrt(squaredDist) - radius;

        float interactionForce = A * Mathf.Exp(-distToNearestObs / B);

        minDistVector.Normalize();
        minDistVector.y = 0;
        Vector3 obsInteractForce = interactionForce * minDistVector.normalized;

        return obsInteractForce;

    }

}
