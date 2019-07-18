using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DestinationReached
{
    None,
    Destroy, // Imidiately destroy agent when reached
    TOBEIMPLEMENTEDTurnBack, // TODO: Return to the starting position using given waypoints
    TOBEIMPLEMENTEDGoToStart // TODO: Go to starting position imidiately after reached
}

public class PedestrianEnd : PedestrianDest
{

    public DestinationReached destinationReached = DestinationReached.None;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
