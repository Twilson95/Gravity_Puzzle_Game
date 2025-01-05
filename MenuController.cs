using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject menuPanel; // Assign this in the inspector
    private Animator menuAnimator;
    private bool isMenuOpen = false;
    public GameObject UIMenu;

    void Start()
    {
        menuAnimator = menuPanel.GetComponent<Animator>();
    }

    public void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen;
        if (isMenuOpen)
        {
            menuAnimator.Play("menuExpansion");
        }
        else
        {
            menuAnimator.Play("menuRetraction");
        }
    }

    public void OpenLevelSelect()
    {
        UIMenu.SetActive(true);
    }
}