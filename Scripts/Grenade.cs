using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Grenade : MonoBehaviour
{

   public Rigidbody body;
   public Transform startPos;
    int strengthOfThrow = 50;
    public Transform player;
    [HideInInspector]
    public bool throwing = false;
    public VisualEffect impactParticle;
    ParticleSystem imPart;
    bool landed = false;
    Area currentScene;

    public  void Start()
    {
        gameObject.SetActive(false);
        if (impactParticle)
        {
            impactParticle.Stop();
            imPart = impactParticle.GetComponent<ParticleSystem>();
        }
        body.isKinematic = true;
    }


    public void Throw( float strengthOfThrow, Area current)
    {
        Vector3 origin = player.transform.forward * strengthOfThrow;
        body.isKinematic = false;
        transform.position = startPos.position;
        transform.rotation = startPos.rotation;
        gameObject.SetActive(true);
        body.AddForce(origin);
        throwing = true;
        currentScene = current;
    }


    public virtual void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground" && throwing)
        {
            impactParticle.transform.position = transform.position;
            impactParticle.Play();
            gameObject.SetActive(false);
            body.isKinematic = true;
            currentScene.currentOxygenProd += currentScene.oxygenRegenTick;
            throwing = false;
        }
        /*if (collision.gameObject.tag == "Tree" && throwing)
        {
            collision.gameObject.GetComponent<OxyObject>().Hit();
            if (impactParticle)
            {
                impactParticle.Play();
                impactParticle.transform.position = transform.position;
                imPart.Play();
            }
            gameObject.SetActive(false);
            body.isKinematic = true;
            throwing = false;
        }*/
    }

}
