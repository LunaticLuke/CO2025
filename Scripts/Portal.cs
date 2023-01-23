using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Portal : MonoBehaviour
{
    [HideInInspector]
    public Transform destination;
    int destNum;
    public Animator anim;
    public VisualEffect effect;

    private void Start()
    {
        gameObject.SetActive(false);

    }

    public void Teleport(GameObject player)
    {
        player.transform.position = destination.position;
    }

    public void CreatePortal(Transform positionToSpawn)
    {
        Vector3 targetPosition = new Vector3(positionToSpawn.position.x, positionToSpawn.position.y + 1, positionToSpawn.position.z);
        transform.position = targetPosition;

        gameObject.SetActive(true);
        anim.SetTrigger("Spawn");
        //effect.Play();
    }

    public void ClosePortal()
    {
        anim.SetTrigger("Despawn");
        //effect.Stop();
    }

    private void Update()
    {
       
    }

}
