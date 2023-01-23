using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PortalGun : Grenade
{
    [System.Serializable]
    public struct Destination
    {
        public Transform Point;
        public string LocationName;
        public GameObject Area;
        public Area area;
    }

    [HideInInspector]
    public  int destValue;
    public Portal portalClass;
    [SerializeField]
    public Destination[] destinationPoints;
    public Text destText;
    [HideInInspector]
    public Destination currentDest;
    public SoundtrackManager soundManager;

    private void Awake()
    {
        portalClass.destination = destinationPoints[2].Point;
        currentDest = destinationPoints[2];
        destValue = 2;
        destText.text = destinationPoints[2].LocationName;
        player.transform.position = currentDest.Point.transform.position;
        HandleScenes();
    }

    public void SelectDestination(int choice, bool unlocked)
    {
        if (choice == 4)
        {
            //Checking China Has Been Unlocked
            if(unlocked)
            {
                portalClass.destination = destinationPoints[3].Point;
                currentDest = destinationPoints[3];
                destValue = 3;
                destText.text = destinationPoints[choice - 1].LocationName;
            }
        }
        else
        {
            //Set the destination equal to the choice - 1 (to suit arrays)
            portalClass.destination = destinationPoints[choice - 1].Point;
            currentDest = destinationPoints[choice - 1];
            destValue = choice - 1;
            destText.text = destinationPoints[choice - 1].LocationName;
        }
    }

   

    public void HandleScenes()
    {
        for(int i = 0; i < destinationPoints.Length;i++)
        {
            if(i == destValue)
            {
                destinationPoints[i].Area.SetActive(true);
            }
            else
            {
                destinationPoints[i].Area.SetActive(false);
            }
        }
    }

    public override void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground" && throwing)
        {
            portalClass.CreatePortal(transform);
            body.isKinematic = true;
            gameObject.SetActive(false);
            portalClass.transform.eulerAngles = new Vector3(0,player.eulerAngles.y,0);
            throwing = false;
        }
    }

}
