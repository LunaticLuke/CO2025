using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class projector : MonoBehaviour
{

    public DecalProjector decal;
    public int speed = 1;
    // Start is called before the first frame update
    
    // Update is called once per frame
    void Update()
    {
        decal.fadeFactor = Mathf.PingPong(Time.time * speed, 1);  
    }
}
