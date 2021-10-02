using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{

    public Tank tank { get; private set; }
    
    void Start()
    {
        tank = GetComponentInParent<Tank>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
