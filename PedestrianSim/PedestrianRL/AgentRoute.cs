using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentRoute : MonoBehaviour
{
    public PedestrianDest[] activeRoute;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmosSelected()
    {
        // Draws a blue line from this transform to the target
        Gizmos.color = Color.red;
        for (int i = 0; i < activeRoute.Length - 1; i++)
            Gizmos.DrawLine(activeRoute[i].transform.position, activeRoute[i + 1].transform.position);
    }
}
