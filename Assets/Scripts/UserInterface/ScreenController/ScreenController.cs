using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenController : MonoBehaviour
{
    public void ScreenState_Button_Clicked()
    {
        if (Screen.fullScreen)
        {
            Screen.fullScreen = false;
        }
        else
        {
            Screen.SetResolution(1920, 1000, true);
            Screen.fullScreen = true;
        }
    }
}
