using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class PedestrianPath : MonoBehaviour
{
    public GameObject pedestrianPrefab; // The pedestrian AI third-person controller for spawning

    public GameObject pedestrianStartNode; 
    public PedestrianDest[] pedestrianNodes;
    public PedestrianEnd pedestrianEndNode;

    [HideInInspector] public float currentSpeed = 0;
    private NavMeshAgent pedestrianNavMesh;
    private AICharacterControl pedestrianControl;

    private GameObject pedestrianCharacter;

    private bool isWaiting = false;
    private int currentNode = 0;
    private PedestrianDest nextNode;

    const float DESTINATION_PROXIMITY = 0.8f;


    // Start is called before the first frame update
    void Start()
    {
        // Spawn new pedestrian
        SpawnNewPedestrian();

        // Hide start node
        pedestrianStartNode.GetComponent<Renderer>().enabled = false;
    }

  

    // Update is called once per frame
    public void FixedUpdate()
    {
        SetNextNode();
        CheckWaiting();

        // Set target
        SetTarget(nextNode.transform);

    }

    public GameObject SpawnNewPedestrian()
    {
        pedestrianCharacter = (GameObject)Instantiate(pedestrianPrefab, pedestrianStartNode.transform.position, Quaternion.identity, this.transform);
        pedestrianNavMesh = pedestrianCharacter.GetComponent<NavMeshAgent>();
        pedestrianControl = pedestrianCharacter.GetComponent<AICharacterControl>();
        pedestrianCharacter.gameObject.tag = "Pedestrian";

        // Go to the first node 
        if (pedestrianNodes.Length == 0)
            nextNode = pedestrianEndNode;
        else
            nextNode = pedestrianNodes[0];
        currentSpeed = nextNode.speed;

        return pedestrianCharacter;
    }

    public void SetNextNode()
    {
        if (!pedestrianCharacter) return;

        // Check distant from character to next node
        if (Vector3.SqrMagnitude(pedestrianCharacter.transform.position - nextNode.transform.position) < DESTINATION_PROXIMITY) // Reached next destination node
        {
            if (nextNode == pedestrianEndNode) // Reach the end
            {
                if (pedestrianEndNode.destinationReached == DestinationReached.Destroy) { Destroy(pedestrianCharacter); }
                else if (pedestrianEndNode.destinationReached == DestinationReached.TOBEIMPLEMENTEDTurnBack) { } //TODO: To be implemented
                else if (pedestrianEndNode.destinationReached == DestinationReached.TOBEIMPLEMENTEDGoToStart) { } //TODO: To be implemented
                else
                    pedestrianControl.target = null;
            }
            else
                currentNode++;

            // Change to next node 
            if (currentNode < pedestrianNodes.Length)
                nextNode = pedestrianNodes[currentNode];
            else
                nextNode = pedestrianEndNode;

            // Set speed to next node's speed;
            currentSpeed = nextNode.speed;
        }
    }

    public PedestrianDest GetNextNode()
    {
        return nextNode;
    }

    public GameObject GetPedestrianCharacter()
    {
        return pedestrianCharacter;
    }

    public void SetTarget(Transform t)
    {
        if (!pedestrianCharacter) return;

        pedestrianControl.target = t;

        if (isWaiting)
            pedestrianNavMesh.speed = 0;
        else
            pedestrianNavMesh.speed = currentSpeed;
    }

    public void CheckWaiting()
    {
        if (!pedestrianCharacter) return;

        // Start waiting if waitBeforeMoving > 0
        if (nextNode.waitBeforeMoving > 0)
            StartCoroutine(Wait(nextNode.waitBeforeMoving));

    }

    IEnumerator Wait(float durationInSecond)
    {
        yield return new WaitForSecondsRealtime(0.3f);
        isWaiting = true;
        yield return new WaitForSecondsRealtime(durationInSecond);
        isWaiting = false;
    }

    void OnDrawGizmosSelected()
    {
        // Draws a blue line from this transform to the target
        Gizmos.color = Color.cyan;
        if (pedestrianNodes.Length == 0)
        {
            Gizmos.DrawLine(pedestrianStartNode.transform.position, pedestrianEndNode.transform.position);
        }
        else
        {
            Gizmos.DrawLine(pedestrianStartNode.transform.position, pedestrianNodes[0].transform.position);
            for (int i = 0; i < (pedestrianNodes.Length - 1); i++)
                Gizmos.DrawLine(pedestrianNodes[i].transform.position, pedestrianNodes[i + 1].transform.position);
            Gizmos.DrawLine(pedestrianNodes[pedestrianNodes.Length - 1].transform.position, pedestrianEndNode.transform.position);
        }
    }
}
