using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{

   



    [TextArea(5,5)]
    public string[] tips;
    public GameObject Blur;
    public GameObject PauseMenu;
    public Text tipText;
    int currentTip = 0;
    int currentMandTip = 0;
    public GameObject popUp;
    public Text popUpText;
    [TextArea(5,5)]
    public string[] mandTips;
    public GameObject PlayerUI;
    

    // Start is called before the first frame update
    void Start()
    {
        Blur.SetActive(false);
        PauseMenu.SetActive(false);
        popUp.SetActive(true);
        PlayerUI.SetActive(false);
        DisplayTip();
    }

    public void Pause()
    {
        Blur.SetActive(true);
        PauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        PlayerUI.SetActive(false);
        Cursor.visible = true;
        FPSController.isPaused = true;
    }

    public void Unpause()
    {
        Blur.SetActive(false);
        PauseMenu.SetActive(false);
        PlayerUI.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        FPSController.isPaused = false;
    }

    private void Update()
    {
        if(!FPSController.isPaused)
        {
            PauseMenu.SetActive(false);
        }
    }

    public void MoveRight()
    {
        if (currentTip + 1 < tips.Length)
        {
            currentTip++;
        }
        else
        {
            currentTip = 0;
        }
        DisplayTip();
    }

    public void MoveLeft()
    {
        if(currentTip - 1 > 0)
        {
            currentTip--;
        }
        else
        {
            currentTip = tips.Length - 1;
        }
        DisplayTip();
    }
    
    void DisplayTip()
    {
        tipText.text = tips[currentTip];
    }

   public void popUpTip()
    {
        Blur.SetActive(true);
        popUpText.text = mandTips[currentMandTip];
        currentMandTip++;
        popUp.SetActive(true);
        
        PlayerUI.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        FPSController.isPaused = true;
        //Time.timeScale = 0;
    }

    public void CloseTip()
    {
        Blur.SetActive(false);
        popUp.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PlayerUI.SetActive(true);
        DisplayTip();
        FPSController.isPaused = false;
        //Time.timeScale = 1;
    }

    public void CloseApp()
    {
        Application.Quit();
    }
}
