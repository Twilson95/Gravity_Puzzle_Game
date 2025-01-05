using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;


public class VerticalPopulator : MonoBehaviour
{
    public GameObject verticalElementPrefab; // Assign the GridElement prefab in the Inspector
    public List<string> sectionList = new List<string>(); // Set the number of elements you want to create
    public VerticalLayoutGroup verticalLayoutGroup;

    private void Start()
    {
        PopulateVerticalLayout();
    }

    void PopulateVerticalLayout()
    {
        foreach(string sectionName in sectionList)
        {
            GameObject newElement = Instantiate(verticalElementPrefab, transform);

            // If using TextMeshPro
            TMP_Text tmpTextComponent = newElement.GetComponentInChildren<TMP_Text>();
            if (tmpTextComponent != null)
            {
                tmpTextComponent.text = sectionName;
            }

            VerticalElement verticalElement = newElement.GetComponent<VerticalElement>();
            if (verticalElement != null)
            {
                verticalElement.Initialize(sectionName);
            }

        }
    }

}
