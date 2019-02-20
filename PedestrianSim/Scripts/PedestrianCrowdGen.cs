using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianCrowdGen : MonoBehaviour
{
    public int maxNoOfInstances = 10; // -1: Unlimited. TODO: To be implemented later
    public float minSpawnTime = 0; // Minimum seconds between every spawn. TODO: To be implemented later
    public float maxSpawnTime = 10f; // Maximum seconds between every spawn. TODO: To be implemented later
    //public bool randomSpawning = true; 

    private int currentNoOfInstances = 0;
    private bool isWaitingForSpawn = false;
    private bool isWaitingForBoid = false;

    public PedestrianCrowdPath[] pedestrianPathOriginal; // List of original paths
    private List<GameObject> pedestrianPathInstances; // List of generated paths (clone)
    private List<GameObject> allPedestrians;

    public bool useBoidMovement = true;
    public float BOID_THINKING_RATE = 0.5f;
    public float BOID_SQR_MAGNITUDE = 16f;
    public float BOID_AVOIDANCE_RATE = 0.9f;
    public float BOID_SPEEDMATCH_RATE = 0.2f;

    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        pedestrianPathInstances = new List<GameObject>();
        allPedestrians = new List<GameObject>();
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Spawning multiple instances until maximum number of instances reached
        while ((currentNoOfInstances < maxNoOfInstances || maxNoOfInstances == -1) &&
               !isWaitingForSpawn)
        {
            // Clone a new path
            GameObject pathTemplate = pedestrianPathOriginal[Random.Range(0, pedestrianPathOriginal.Length)].gameObject; // Choose a random original path to be cloned
            GameObject clonedPath = Instantiate(pathTemplate, pathTemplate.transform.position, Quaternion.identity, this.transform);
            pedestrianPathInstances.Add(clonedPath);

            // Spawn new pedestrian
            GameObject go = clonedPath.GetComponent<PedestrianCrowdPath>().SpawnNewPedestrian();
            //allPedestrians.Add(go);

            StartCoroutine(SpawnWaiting(Random.Range(minSpawnTime, maxSpawnTime)));
            //currentNoOfInstances++;
        }

        allPedestrians.Clear();
        currentNoOfInstances = 0;
        foreach (GameObject pedPathIns in pedestrianPathInstances)
        {
            if (!pedPathIns.GetComponent<PedestrianCrowdPath>().GetPedestrianCharacter())
            {
                pedestrianPathInstances.Remove(pedPathIns);
                break;
            }
            PedestrianCrowdPath p = pedPathIns.GetComponent<PedestrianCrowdPath>();
            p.SetNextNode();
            p.CheckWaiting();
            allPedestrians.Add(p.GetPedestrianCharacter());
            currentNoOfInstances++;

        }

        if (useBoidMovement)
        {
            // Start BOID movement
            SetPedestrianTargets();
            StartCoroutine(BoidWaiting(BOID_THINKING_RATE));
        }
        else
        {
            foreach (GameObject go in pedestrianPathInstances)
            {
                PedestrianCrowdPath p = go.GetComponent<PedestrianCrowdPath>();
                p.SetTarget(p.GetNextNode().transform);
            }
        }

        // Check if objects are destroyed
        //TODO

    }

    public void SetPedestrianTargets()
    {
        if (isWaitingForBoid) return;
        foreach (GameObject go in allPedestrians)
        {
            //if (!go) return;
            // Calculate next target
            Vector3 direction = new Vector3();

            PedestrianCrowdPath crowdPath = go.GetComponentInParent<PedestrianCrowdPath>();

            direction = crowdPath.GetNextNode().transform.position - go.transform.position;

            float speedSum = 0f;
            float noOfBoidObj = 0;

            // BOID movement: Staying away from other pedestrians in a squared magnitude of BOID_SQR_MAGNITUDE
            foreach (GameObject boidObj in allPedestrians)
            {
                // Select all GameObject within range and not self
                if (Vector3.SqrMagnitude(go.transform.position - boidObj.transform.position) < BOID_SQR_MAGNITUDE
                    && boidObj != go)
                {
                    direction = direction.normalized;

                    direction -= (boidObj.transform.position - go.transform.position) * BOID_AVOIDANCE_RATE;

                    // Calculate Speed to match surrouding
                    speedSum += boidObj.GetComponentInParent<PedestrianCrowdPath>().currentSpeed;
                    noOfBoidObj++;
                }
            }

            // Include current player to calculation
            if (player)
            {

                if (Vector3.SqrMagnitude(go.transform.position - player.transform.position) < BOID_SQR_MAGNITUDE
                     && player != go)
                {
                    direction -= (player.transform.position - go.transform.position) * BOID_AVOIDANCE_RATE;
                    direction -= (player.transform.position - go.transform.position) * BOID_AVOIDANCE_RATE;
                }
            }

            // Set target toward direction vector
            crowdPath.currentTarget.transform.position = go.transform.position + direction;
            crowdPath.SetTarget(crowdPath.currentTarget.transform);
            if (noOfBoidObj > 0)
            {
                crowdPath.currentSpeed += (speedSum / noOfBoidObj - crowdPath.currentSpeed) * BOID_SPEEDMATCH_RATE;
            }

        }
    }

    IEnumerator SpawnWaiting(float durationInSecond)
    {
        isWaitingForSpawn = true;
        yield return new WaitForSecondsRealtime(durationInSecond);
        isWaitingForSpawn = false;
    }

    IEnumerator BoidWaiting(float durationInSecond)
    {
        isWaitingForBoid = false;
        yield return new WaitForSecondsRealtime(durationInSecond);
        isWaitingForBoid = true;
    }

}
