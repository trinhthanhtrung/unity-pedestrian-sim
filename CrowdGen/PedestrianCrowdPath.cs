using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianCrowdPath : PedestrianPath
{
    [HideInInspector] public GameObject currentTarget;

    // Start is called before the first frame update
    void Start()
    {
        // Hide start node
        pedestrianStartNode.GetComponent<Renderer>().enabled = false;

        currentTarget = new GameObject("Current Target");
        currentTarget.transform.parent = this.transform;
    }

    // Update is called once per frame
    new void FixedUpdate()
    {

    }
}
