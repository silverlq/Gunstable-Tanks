using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundWrap : MonoBehaviour
{
    private Vector3 scaleVector;
    // Start is called before the first frame update
    void Start()
    {
        scaleVector = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        var xDiff = LevelManager.player.transform.position.x - transform.position.x;
        var zDiff = LevelManager.player.transform.position.z - transform.position.z;
        
        if (xDiff > scaleVector.x)
        {
            transform.position += new Vector3(scaleVector.x,0f,0f);
        }
        else if (xDiff < -scaleVector.x)
        {
            transform.position -= new Vector3(scaleVector.x, 0f, 0f);
        }

        if (zDiff > scaleVector.z)
        {
            transform.position += new Vector3(0f, 0f,scaleVector.z);
        }
        else if (zDiff < -scaleVector.z)
        {
            transform.position -= new Vector3(0f, 0f, scaleVector.z);
        }
    }
}
