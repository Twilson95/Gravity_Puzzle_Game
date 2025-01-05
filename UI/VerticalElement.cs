using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VerticalElement : MonoBehaviour
{
    private string sectionName;
    public GameObject gridContainer;
    public GameObject generalMenu;
    private Sprite imageSprite;

    public void Initialize(string sectionName)
    {
        this.sectionName = sectionName;
        GetComponent<Button>().onClick.AddListener(OnButtonClick);

        Image sectionImage = transform.Find("PlanetImage").GetComponent<Image>();
        Image titleTextImage = transform.Find("TitleText/TextPlanetImage").GetComponent<Image>();
        TMP_Text completionText = transform.Find("CompletionText").GetComponent<TMP_Text>();

        SaveLoadScript saveLoadScript = FindObjectOfType<SaveLoadScript>();
        int numberOfCompletedLevelsInSection = 0;
        int numberOfLevelsInSection = saveLoadScript.GetNumberOfLevelsForSection(sectionName);
        completionText.text = numberOfCompletedLevelsInSection.ToString() + "/" + numberOfLevelsInSection.ToString();

        imageSprite = Resources.Load<Sprite>($"Planets/{sectionName}");
        if (imageSprite != null)
        {
            sectionImage.sprite = imageSprite;
            titleTextImage.sprite = imageSprite;
            // completionTextImage.sprite = imageSprite;
        }
    }

    private void OnButtonClick()
    {
        GameObject gridContainer = GameObject.Find("GridContainer");
        GameObject generalMenu = GameObject.Find("Menus");

        if (gridContainer != null)
        {
            // Get the GridPopulator component and call PopulateGrid with the section name
            GridPopulator gridPopulator = gridContainer.GetComponent<GridPopulator>();
            if (gridPopulator != null)
            {
                gridPopulator.PopulateLevelGrid(sectionName, imageSprite);
            }
            SlideMenus slideMenus = generalMenu.GetComponent<SlideMenus>();
            slideMenus.SlideIn();
            
        }
        else
        {
            Debug.LogError("GridContainer GameObject not found!");
        }

    }
}

