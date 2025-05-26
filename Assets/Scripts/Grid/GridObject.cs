using System.Collections.Generic;

public class GridObject {
    GridPosition gridPosition;
    List<BaseGridItem> gridItemsList;
    DestroyState destroyState;

    bool isSafe;

    enum DestroyState {
        None,
        BeingDestroyed,
        Destroyed
    };

    public GridObject(GridPosition gridPosition)
    {
        this.gridPosition = gridPosition;
        gridItemsList = new List<BaseGridItem>();
    }

    public void AddGridItem(BaseGridItem gridItem)
    {
        gridItemsList.Add(gridItem);
    }

    public List<BaseGridItem> GetGridItemsList()
    {
        return gridItemsList;
    }

    public void RemoveGridItem(BaseGridItem gridItem)
    {
        gridItemsList.Remove(gridItem);
    }

    public void DestroyGridItems()
    {
        for (int i = gridItemsList.Count - 1; i >= 0; i--)
        {
            BaseGridItem gridItem = gridItemsList[i];
            gridItem.HandleGridParentDeath();
            RemoveGridItem(gridItem);
        }
    }

    public void SetIsBeingDestroyed()
    {
        destroyState = DestroyState.BeingDestroyed;
    }

    public void SetIsDestroyed()
    {
        destroyState = DestroyState.Destroyed;
    }

    public bool IsBeingDestroyed()
    {
        return destroyState == DestroyState.BeingDestroyed;
    }
    public bool IsDestroyed()
    {
        return destroyState == DestroyState.Destroyed;
    }

    public override string ToString()
    {
        string gridObjectString = "";
        foreach (BaseGridItem gridItem in gridItemsList)
        {
            gridObjectString += gridItem.name + "\n";
        }

        return $"{gridPosition.ToString()}\n{gridObjectString}";
    }


    public bool IsSafe()
    {
        return isSafe;
    }

    public void SetIsSafe(bool isSafe)
    {
        this.isSafe = isSafe;
        for (int i = gridItemsList.Count - 1; i >= 0; i--)
        {
            BaseGridItem gridItem = gridItemsList[i];
            gridItem.HandleGridParentIsSafeChange(isSafe);
        }
    }
}
