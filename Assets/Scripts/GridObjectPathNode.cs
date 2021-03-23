using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObjectPathNode : ObjectTypes
{

    public Grid<GridObjectPathNode> grid;
    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public int fCost;

    public bool isWalkable;
    public GridObjectPathNode cameFromNode;
    public PlacedObject placedObject;
    public ObjectTypes.objectType gridObjectType;

    public GridObjectPathNode(Grid<GridObjectPathNode> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
        isWalkable = true;
    }

    public void CalculateFCost()
    {
        fCost = hCost + gCost;
    }

    public void SetIsWalkable(bool isWalkable)
    {
        this.isWalkable = isWalkable;
        grid.TriggerGridObjectChanged(x, y);
    }

    public void SetPlacedObject(PlacedObject placedObject)
    {
        this.placedObject = placedObject;
        SetIsWalkable(placedObject == null);
        if (placedObject == null) SetGridObjectType(ObjectTypes.objectType.None);
        else SetGridObjectType(placedObject.GetGridObjectType());
        grid.TriggerGridObjectChanged(x, y);
    }

    public PlacedObject GetPlacedObject()
    {
        return placedObject;
    }

    public void ClearPlacedObject()
    {
        placedObject = null;
        this.isWalkable = (placedObject == null);
        grid.TriggerGridObjectChanged(x, y);
    }

    public bool CanBuild()
    {
        return placedObject == null;
    }

    public void SetGridObjectType(ObjectTypes.objectType type)
    {
        if (type == null) this.gridObjectType = ObjectTypes.objectType.None;
        else this.gridObjectType = type;
        grid.TriggerGridObjectChanged(x, y);
    }

    public ObjectTypes.objectType GetGridObjectType()
    {
        return gridObjectType;
    }

    public override string ToString()
    {
        string name = null;
        if (placedObject != null) name = placedObject.name;
        return x + ", " + y + "\n" + gridObjectType ;
    }
}
