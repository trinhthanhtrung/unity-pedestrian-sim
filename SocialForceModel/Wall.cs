using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private float wallLength;
    private float wallAngleR;
    private Vector3 start;
    private Vector3 end;

    // Start is called before the first frame update
    void Start()
    {
        wallAngleR = -Mathf.Deg2Rad * this.transform.eulerAngles.y;
        wallLength = this.transform.localScale.x;
        start = new Vector3(this.transform.position.x + 0.5f * wallLength * Mathf.Cos(wallAngleR),
                            this.transform.position.y,
                            this.transform.position.z + 0.5f * wallLength * Mathf.Sin(wallAngleR));
        end = new Vector3(this.transform.position.x - 0.5f * wallLength * Mathf.Cos(wallAngleR),
                          this.transform.position.y,
                          this.transform.position.z - 0.5f * wallLength * Mathf.Sin(wallAngleR));
    }

    public Vector3 GetNearestPoint(Vector3 pos)
    {
        Vector3 relativeEnd, relativePos, relativeEndScaled, relativePosScaled;
        float dotProduct;
        Vector3 nearestPoint;

        // Relative vector to start position
        relativeEnd = end - start;   
        relativePos = pos - start; 

        
        relativeEndScaled = relativeEnd.normalized;
        relativePosScaled = relativePos * (1.0f / Vector3.Magnitude(relativeEnd));

        // Dot Product of Scaled Vectors
        dotProduct = Vector3.Dot(relativeEndScaled, relativePosScaled);

        if (dotProduct < 0.0)       // Position of agent is located before wall's 'start'
            nearestPoint = start;
        else if (dotProduct > 1.0)  // Position of agent is located after wall's 'end'
            nearestPoint = end;
        else                        // Position of agent is located between wall's 'start' and 'end'
            nearestPoint = (relativeEnd * dotProduct) + start;

        return nearestPoint;
    }

    public float GetSqrDistanceTo(Vector3 pos)
    {
        Vector3 vectorToWall = pos - GetNearestPoint(pos);

        return Vector3.SqrMagnitude(vectorToWall);
    }

    public float GetSqrDistanceTo(Transform transform) => GetSqrDistanceTo(transform.position);

}
