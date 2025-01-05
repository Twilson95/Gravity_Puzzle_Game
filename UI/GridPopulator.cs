using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;


public class GridPopulator : MonoBehaviour
{
    public GameObject gridElementPrefab; // Assign the GridElement prefab in the Inspector
    public GridLayoutGroup gridLayoutGroup;
    public float slideDuration = 0.5f; // Duration of the slide-in animation
    public RectTransform gridContainerRectTransform;

    private Vector2 initialPosition;
    private Vector2 targetPosition;
    private int numberOfElements;

    private void Start()
    {
        initialPosition = gridContainerRectTransform.anchoredPosition;
        targetPosition = new Vector2(0, initialPosition.y);
        // PopulateLevelGrid();
        // AdjustCellSize();
    }

    public void PopulateLevelGrid(string sectionName, Sprite planetImage)
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        SaveLoadScript saveLoadScript = FindObjectOfType<SaveLoadScript>();
        numberOfElements = saveLoadScript.GetNumberOfLevelsForSection(sectionName);

        for (int i = 1; i < numberOfElements + 1; i++)
        {
            GameObject newElement = Instantiate(gridElementPrefab, transform);

            // If using TextMeshPro
            TMP_Text tmpTextComponent = newElement.GetComponentInChildren<TMP_Text>();
            if (tmpTextComponent != null)
            {
                tmpTextComponent.text = i.ToString();
            }

            Image levelImage = newElement.GetComponentInChildren<Image>();
            if (levelImage != null)
            {
                levelImage.sprite = planetImage;
            }

            GridElement gridElement = newElement.GetComponent<GridElement>();
            if (gridElement != null)
            {
                gridElement.Initialize(sectionName, i);
            }
        }
        AdjustCellSize();
    }

    public void SlideIn()
    {
        StopAllCoroutines();
        StartCoroutine(SlideInCoroutine());
    }

    private IEnumerator SlideInCoroutine()
    {
        float elapsedTime = 0f;

        while (elapsedTime < slideDuration)
        {
            gridContainerRectTransform.anchoredPosition = Vector2.Lerp(initialPosition, targetPosition, elapsedTime / slideDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gridContainerRectTransform.anchoredPosition = targetPosition;
    }

    void AdjustCellSize()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        float width = rectTransform.rect.width - gridLayoutGroup.padding.left - gridLayoutGroup.padding.right;
        float height = rectTransform.rect.height - gridLayoutGroup.padding.top - gridLayoutGroup.padding.bottom;

        int columns = gridLayoutGroup.constraintCount;
        int rows = Mathf.CeilToInt(numberOfElements/columns);

        float cellWidth = (width - (gridLayoutGroup.spacing.x * (columns-1))) / (columns);

        // Set cell height to be the same as cell width
        float cellHeight = cellWidth;

        gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);

        // float cellHeight = (height - (gridLayoutGroup.spacing.y * (rows-1))) / (rows);

        // gridLayoutGroup.cellSize = new Vector2(cellWidth, cellHeight);
    }
}
