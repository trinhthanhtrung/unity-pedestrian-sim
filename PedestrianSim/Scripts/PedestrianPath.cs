using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class PedestrianPath : MonoBehaviour
{
    public GameObject pedestrianPrefab; // The pedestrian AI third-person controller for spawning
    public int numberOfInstances = 1; // -1: Unlimited. TODO: To be implemented later
    public float minSpawnTime = 0; // Minimum seconds between every spawn. TODO: To be implemented later
    public float maxSpawnTime = 10f; // Maximum seconds between every spawn. TODO: To be implemented later

    public GameObject pedestrianStartNode; 
    public PedestrianDest[] pedestrianNodes;
    public PedestrianEnd pedestrianEndNode;

    private NavMeshAgent pedestrianNavMesh;
    private AICharacterControl pedestrianControl;
    private GameObject pedestrianCharacter;
    private bool isWaiting = false;
    private int currentNode = 0;
    PedestrianDest nextNode;

    const float DESTINATION_PROXIMITY = 0.8f;

    // Start is called before the first frame update
    void Start()
    {

        // Spawn new pedestrian
        pedestrianCharacter = (GameObject)Instantiate(pedestrianPrefab, pedestrianStartNode.transform.position, Quaternion.identity);
        pedestrianNavMesh = pedestrianCharacter.GetComponent<NavMeshAgent>();
        pedestrianControl = pedestrianCharacter.GetComponent<AICharacterControl>();

        // Go to the first node
        if (pedestrianNodes.Length == 0)
            nextNode = pedestrianEndNode;
        else
            nextNode = pedestrianNodes[0];
        
        // Hide start node
        pedestrianStartNode.GetComponent<Renderer>().enabled = false;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!pedestrianCharacter) return;

        // Check distant from character to next node
        if (Vector3.SqrMagnitude(pedestrianCharacter.transform.position - nextNode.transform.position) < DESTINATION_PROXIMITY) // Reached next destination node
        {
            if (nextNode == pedestrianEndNode) // Reach the end
            {
                if (pedestrianEndNode.destinationReached == DestinationReached.Destroy) { Destroy(pedestrianCharacter); }
                else if (pedestrianEndNode.destinationReached == DestinationReached.TurnBack) { }
                else if (pedestrianEndNode.destinationReached == DestinationReached.GoToStart) { }
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

            // Start waiting if waitBeforeMoving > 0
            if (nextNode.waitBeforeMoving > 0)
                StartCoroutine(Wait(nextNode.waitBeforeMoving));
        }

        pedestrianControl.target = nextNode.transform;
        if (isWaiting)
            pedestrianNavMesh.speed = 0;
        else
            pedestrianNavMesh.speed = nextNode.speed;

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
