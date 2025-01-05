using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GridElement : MonoBehaviour
{
    private int levelNumber;
    private string sectionName;

    public void Initialize(string sectionName, int levelNumber)
    {
        this.levelNumber = levelNumber;
        this.sectionName = sectionName;

        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        LevelManager levelManager = FindObjectOfType<LevelManager>();
        levelManager.LoadLevel(sectionName, levelNumber);
        GameObject UIMenu = GameObject.Find("UI");
        UIMenu.SetActive(false);
    }
}

