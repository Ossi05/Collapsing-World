using TMPro;
using UnityEngine;

public class GridDebugObject : MonoBehaviour {
    [SerializeField] TextMeshPro textMeshPro;
    GridObject gridObject;

    public void SetGridObject(GridObject gridObject)
    {
        this.gridObject = gridObject;
        textMeshPro.text = gridObject.ToString();
    }

    void Update()
    {
        textMeshPro.text = gridObject.ToString();
        if (gridObject.GetIsDestroyed())
        {
            textMeshPro.color = Color.red;
        }
        if (gridObject.GetIsSafe())
        {
            textMeshPro.color = Color.green;
        }
        // We could add here if the path is safe and it's also destroyable
        else
        {
            textMeshPro.color = Color.white;
            textMeshPro.alpha = 0f;
        }

    }
}
