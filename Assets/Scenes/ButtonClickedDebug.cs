using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClickedDebug : MonoBehaviour
{
    public void OnButtonClick()
    {
        Debug.Log("Button Clicked: " + gameObject.name);
    }
}
