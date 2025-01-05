using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButtonColour : MonoBehaviour
{
    private Button button;
    private bool isOn = false;
    public Color defaultColour;
    public Color activeColour;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ToggleColor);
    }

    void ToggleColor()
    {
        isOn = !isOn;
        if (isOn){
            button.image.color = activeColour;
        }
        else{
            button.image.color = defaultColour;
        }
    }
}
