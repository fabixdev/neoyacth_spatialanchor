//allineare oggett

using UnityEngine;


public class Allineatore : MonoBehaviour
{
    public int rows = 2; // Numero di righe nella griglia
    public int columns = 2; // Numero di colonne nella griglia
    public Vector2 cellSize = new Vector2(100, 100); // Dimensioni delle celle della griglia
    public Vector2 spacing = new Vector2(10, 10); // Spaziatura tra le celle della griglia
    public Vector2 startOffset = new Vector2(10, 10); // Offset iniziale della griglia

    void Start()
    {
        ArrangeGrid();
    }

    void ArrangeGrid()
    {
        int childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);

            int row = i / columns;
            int column = i % columns;

            float posX = startOffset.x + (cellSize.x + spacing.x) * column;
            float posY = startOffset.y + (cellSize.y + spacing.y) * row;

            child.localPosition = new Vector3(posX, -posY, child.localPosition.z);
        }
    }

    void OnValidate()
    {
        ArrangeGrid();
    }
}
