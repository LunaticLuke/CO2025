using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.VFX;
using UnityEngine.SceneManagement;

public class FPSController : MonoBehaviour
{

    public struct Sprout
    {
        public Grenade grenade;
        bool used;
    }



    #region FPS Variables
    [Header("Movement Settings")]
    //Character Movement Speed
    public float movementSpeed = 10.0f;
    //Height of player's jump
    public float jumpHeight = 8.0f;
    //Is the player grounded?
    bool grounded = true;
    // Float to store forward/backward translation
    float translation;
    // Float to store the sideways movement
    float straffe;
    [Header("Components")]
    //Animator component for the player
    public Animator anim;
    //THe player's rigidbody component
    public Rigidbody body;
    //The character gameObject
    public Transform character;
    InputValue input;
    public static PrototypeZone controls;
    public Transform cam;
    float deltaTime;
    [Header("Weapon Settings")]
    public PortalGun portalGun;
    int currentWeapon = 1;
    public Grenade[] grenades;
    public bool[] thrown;
    Vector2 scrollValue;
    public int numberOfWeapons = 2;
    public Text weaponText;
    public Animator weaponAnim;
    public GameObject portalGunGO;
    public Renderer co2GunGO;
    public GameObject suckingParticle;
    float maxCapacity = 30;
    float capacityOfCo2 = 0;
    bool hoovering = false;
    public Text co2Text;
    public GameObject capsule;
    public VisualEffect[] effects;
    public VisualEffect[] portalEffects;
    int currentLocation;
    public Slider oxygenSlider;
    public Slider oxygenSlider2;
    public Slider co2Slider;
    public Slider co2Slider2;
    public GameObject hooverImage, portalImage;
    public Text GrenadesText;
    public Text cannisterText;
    public SoundtrackManager soundManager;
    #endregion

    #region Mouse Look Variables
    // Sensitivity variable
    [Header("Mouse Look Settings")]
    public float sensitivityX = 8.0f;
    public float sensitivityY = 0.5f;
    // The smoothing factor applied.
    public float smoothing = 2.0f;
    public float xClamp;
    Vector2 mouseLook;
    Vector2 smoothV;
    Vector2 mouseCoords;
    float xRotation = 0f;
    public float rotationX;
    [Header("Location Settings")]
    public Area[] locations;
    bool visitedBrazil;
    bool unlockedChina;
    public Tutorial tutorial;
    int cannistersOwned = 0;
    public int grenadesAwarded = 3;
    int grenadeAmmo = 0;
    public static bool isPaused = false;
    public AudioSource audioS;
    public AudioSource walking;
    public AudioClip hoover;
    public AudioSource portalG;
    public AudioClip walkGrass, walkPave;


    #endregion

    private void Awake()
    {
        controls = new PrototypeZone();
        controls.Player.Jump.performed += _ => OnJump();
        controls.Player.Fire.performed += ctx => Throw();
        controls.Player.MouseX.performed += ctx => mouseCoords.x = ctx.ReadValue<float>();
        controls.Player.MouseY.performed += ctx => mouseCoords.y = ctx.ReadValue<float>();
        controls.Player.Select1.performed += ctx => portalGun.SelectDestination(1,unlockedChina);
        controls.Player.Select2.performed += ctx => portalGun.SelectDestination(2,unlockedChina);
        controls.Player.Select3.performed += ctx => portalGun.SelectDestination(3,unlockedChina);
        controls.Player.Select4.performed += ctx => portalGun.SelectDestination(4,unlockedChina);
        controls.Player.Scroll.performed += ctx => ChangeWeapon(ctx.ReadValue<Vector2>());
        controls.Player.Grenade.performed += ctx => fireGrenade();
        controls.Player.Hoover.canceled += ctx => stopAbsorbing();
        controls.Player.Hoover.performed += ctx => StartAbsorbing();
        controls.Player.Reload.performed += ctx => EmptyCo2();
        controls.Player.Move.performed += ctx => walking.Play();
        controls.Player.Move.canceled += ctx => walking.Stop();
        controls.Player.Pause.performed += ctx => tutorial.Pause();
        UpdateUI();
        SwapWeapon();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Lock the cursor to centre of screen.
        Cursor.lockState = CursorLockMode.Locked;
        //Make sure the cursor isn't visible.
        Cursor.visible = false;
        suckingParticle.SetActive(false);
        portalEffects[0].Stop();
        currentLocation = 2;
        audioS.volume = 0.5f;
        tutorial.popUpTip();
    }

    // Update is called once per frame
    void Update()
    {



        if (!isPaused)
        {
            Vector2 movement = controls.Player.Move.ReadValue<Vector2>();
            //Determine translation from Vertical controller axis (W and S) - multiply by deltaTime and movement speed
            translation = (movement.y * movementSpeed) * Time.deltaTime;
            //Determine translation from horizontal controller axis (A and D) - multiply by deltaTime and movement speed
            straffe = (movement.x * movementSpeed) * Time.deltaTime;
            //Move the player on the x axis by straffe and on the z axis by translation.
            transform.Translate(straffe, 0, translation);
            ReceiveInput(mouseCoords);

            transform.Rotate(Vector3.up, mouseCoords.x * Time.fixedDeltaTime);

            xRotation -= mouseCoords.y;
            xRotation = Mathf.Clamp(xRotation, -xClamp, xClamp);
            Vector3 targetRotation = transform.eulerAngles;
            targetRotation.x = xRotation;
            cam.eulerAngles = targetRotation;

            cannisterText.text = string.Format("Co2 Cannisters: {0}", cannistersOwned);
            GrenadesText.text = string.Format("Life Bombs: {0}", grenadeAmmo);

            if (currentWeapon == 2)
            {
                if (capacityOfCo2 >= maxCapacity)
                {
                    co2Text.text = "Press R To Empty A Co2 Cannister";
                }
                else
                {
                    co2Text.text = string.Format("C02 Capacity {0} / {1}", capacityOfCo2, maxCapacity);
                }

            }
            else
            {
                if (!portalGun.throwing)
                {
                    portalEffects[0].Stop();
                }
            }

            if (hoovering)
            {
                Throw();
            }

            if (!locations[currentLocation].hq)
            {
                oxygenSlider.gameObject.SetActive(true);
                oxygenSlider2.gameObject.SetActive(true);
                co2Slider.gameObject.SetActive(true);
                co2Slider2.gameObject.SetActive(true);
                oxygenSlider.maxValue = locations[currentLocation].targetOxygenProd;
                oxygenSlider2.maxValue = locations[currentLocation].targetOxygenProd;
                oxygenSlider.value = locations[currentLocation].currentOxygenProd;
                oxygenSlider2.value = locations[currentLocation].currentOxygenProd;
                co2Slider.maxValue = locations[currentLocation].maxCo2Level;
                co2Slider2.maxValue = locations[currentLocation].maxCo2Level;
                co2Slider.value = locations[currentLocation].currentCo2Level;
                co2Slider2.value = locations[currentLocation].currentCo2Level;
            }
            else
            {
                oxygenSlider.gameObject.SetActive(false);
                oxygenSlider2.gameObject.SetActive(false);
                co2Slider.gameObject.SetActive(false);
                co2Slider2.gameObject.SetActive(false);
            }

            locations[0].Update();
            if(unlockedChina)
            {
                locations[3].ableToDeduct = true;
            }
            else
            {
                locations[3].ableToDeduct = false;
            }
            
            if(locations[0].carbonNeutral && locations[3].carbonNeutral)
            {
                SceneManager.LoadScene(2);
            }

            if(currentLocation == 2)
            {
                walking.clip = walkPave;
            }
            else
            {
                walking.clip = walkGrass;
            }
        }
    }

   


    void OnCollisionEnter(Collision other)
    {
        //If the other collider is the ground
        if(other.gameObject.tag == "Ground" && !isPaused)
        {
            //It is now grounded.
            grounded = true;
        }
        if (other.gameObject.tag == "Cannister" && !isPaused)
        {
            other.gameObject.SetActive(false);
            if(!unlockedChina)
            {
                unlockedChina = true;
                tutorial.popUpTip();
            }
            cannistersOwned++;
        }
    }

    void setAnimBool(string animFalse, string animTrue)
    {
        //Set the false anim false
        anim.SetBool(animFalse, false);
        //Set the true anim true.
        anim.SetBool(animTrue, true);
    }

   public PrototypeZone GetControls()
    {
        return controls;
    }
   
    public void ReceiveInput(Vector2 mouseInput)
    {
        mouseCoords.x = mouseInput.x * sensitivityX;
        mouseCoords.y = mouseInput.y * sensitivityY;
    }


    public void OnJump()
    {
        if (grounded && !isPaused)
        {
                Debug.Log("Jumping");
                //Add force to propel the rigidbody upwards.
                body.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
                //No longer grounded.
                grounded = false;
        }
    }

    public void Throw()
    {
        if (!isPaused)
        {
            switch (currentWeapon)
            {
                case 1:

                    if (!portalGun.throwing)
                    {
                        portalG.Play();
                        portalGun.portalClass.ClosePortal();
                        portalGun.Throw(300,null);
                        portalEffects[0].Play();
                    }

                    break;
                case 2:

                    if (capacityOfCo2 < maxCapacity)
                    {
                        movementSpeed = 2.5f;
                        //suckingParticle.SetActive(true);

                        if (locations[currentLocation].currentCo2Level > 0 && !locations[currentLocation].hq)
                        {
                            capacityOfCo2 += 0.25f;
                            locations[currentLocation].currentCo2Level -= 1;
                        }
                    }
                    else
                    {
                        stopAbsorbing();
                    }

                    break;


            }
        }
    }


    public void StartAbsorbing()
    {
        if (currentWeapon == 2  && !isPaused)
        {
            audioS.clip = hoover;
            audioS.Play();
            audioS.loop = true;
            hoovering = true;
            suckingParticle.SetActive(true);
            for (int i = 0; i < effects.Length; i++)
            {
                effects[i].Play();
            }
        }
    }
    public void EmptyCo2()
    {
        if(capacityOfCo2 >= maxCapacity)
        {
            //Cannister
            while(capacityOfCo2 != 0)
            {
                capacityOfCo2--;
               
            }
            capsule.transform.position = co2GunGO.transform.position;
            capsule.SetActive(true);
        }
    }

    public void stopAbsorbing()
    {
        movementSpeed = 5;
        hoovering = false;
        audioS.clip = null;
        audioS.Stop();
        audioS.loop = true;
        if(currentWeapon == 2)
        {
            // suckingParticle.SetActive(false);
            for(int i = 0; i < effects.Length;i++)
            {
                effects[i].Stop();
            }
        }
    }

    void SwapWeapon()
    {
        if (!isPaused)
        {
            switch (currentWeapon)
            {
                case 1:
                    weaponAnim.SetTrigger("Swap");
                    portalGunGO.SetActive(true);
                    portalImage.SetActive(true);
                    hooverImage.SetActive(false);
                    co2GunGO.enabled = false;
                    break;
                case 2:
                    weaponAnim.SetTrigger("Swap");
                    portalGunGO.SetActive(false);
                    portalImage.SetActive(false);
                    hooverImage.SetActive(true);
                    co2GunGO.enabled = true;
                    break;
            }
        }
    }

    void fireGrenade()
{
        if (currentWeapon == 2 && !isPaused && grenadeAmmo != 0)
        {
            for (int i = 0; i < grenades.Length; i++)
            {
                if (!thrown[i])
                {
                    grenades[i].Throw(Random.Range(500, 1000),locations[currentLocation]);
                    grenadeAmmo--;
                    thrown[i] = true;
                    break;
                }
            }
        }
    }

    void UpdateUI()
    {
        switch (currentWeapon)
        {
            case 1:
                weaponText.text = "Portal Gun";
                break;
            case 2:
                weaponText.text = "C02 Isolator";
                break;
        }

    }


    void ChangeWeapon(Vector2 value)
    {
        if (!isPaused)
        {
            switch (currentWeapon)
            {
                case 1:
                    currentWeapon = 2;
                    movementSpeed = 5;
                    portalEffects[0].Stop();
                    break;
                case 2:
                    currentWeapon = 1;
                    for (int i = 0; i < effects.Length; i++)
                    {
                        effects[i].Stop();
                    }
                    movementSpeed = 5;
                    portalEffects[0].Play();
                    break;
            }
            SwapWeapon();
            UpdateUI();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Portal" && !isPaused)
        {
            portalGun.HandleScenes();
            portalGun.portalClass.Teleport(this.gameObject);
            currentLocation = portalGun.destValue;
            portalGun.portalClass.gameObject.SetActive(false);
            if(!locations[currentLocation].hq && !visitedBrazil)
            {
                visitedBrazil = true;
                tutorial.popUpTip();
            }
           StartCoroutine( soundManager.FadeOut(currentLocation));
        }

        if(other.gameObject.tag == "CCS Point" && cannistersOwned > 0 && !isPaused)
        {
            for(int i = 0; i < cannistersOwned; i++)
            {
                cannistersOwned--;
                grenadeAmmo += grenadesAwarded;
            }
        }
        
    }

    
}
