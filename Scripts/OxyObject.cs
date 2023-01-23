using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxyObject : MonoBehaviour
{
    public Renderer rend;
    Color startingColour;
    bool hit = false;
    public float timeToFade = 5.0f;
    float fadeInterval, fadeValue;
    // Start is called before the first frame update


    private void Start()
    {
        startingColour = rend.material.color;
        fadeInterval = (1 / timeToFade);
    }


    // Update is called once per frame
    void Update()
    {
       
        if(hit && fadeValue < 1)
        {
            fadeValue += fadeInterval * Time.deltaTime;
            rend.material.color = Color.Lerp(startingColour, Color.green, fadeValue);
        }
    }

    public void Hit()
    {
        hit = true;
    }
}
