using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PedestrianDest : MonoBehaviour
{

    public float speed = 0.4f; // Pedestrian speed to reach this node
    public float waitBeforeMoving = 0; // Wait time before start moving to node

    public bool hideWhenPlaying = true; // Unset this option only for debugging

    // Start is called before the first frame update
    void Start()
    {
        // Hide helper objects
        if (hideWhenPlaying)
            this.GetComponent<Renderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
