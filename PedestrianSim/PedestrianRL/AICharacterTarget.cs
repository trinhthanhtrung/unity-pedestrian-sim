using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class AICharacterTarget : MonoBehaviour
{
    public Transform target;
    private Vector3 movementVector;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<AICharacterControl>().target = target;
        movementVector = new Vector3(0, 0.5f, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        target.position = this.transform.position + movementVector.normalized * 1.5f;
    }

    // Add a small force to the target;
    public void AddVector(Vector3 movementForce)
    {
        movementVector += movementForce;
    }

    // Reset position
    public void ResetTarget()
    {
        movementVector = new Vector3(0, 0.5f, 0);
    }
}
