using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateScrollDefault : MonoBehaviour
{
    // Start is called before the first frame update
    public void Start()
    {
        Invoke("SetScrollPosition", 0.001f);
    }

    public void SetScrollPosition()
    {
        GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
    }
}
