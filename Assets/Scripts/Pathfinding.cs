using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
{
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    public static Pathfinding Instance { get; private set; }

    private Grid<GridObjectPathNode> grid;
    private List<GridObjectPathNode> openList;
    private List<GridObjectPathNode> closedList;

    public Pathfinding(Grid<GridObjectPathNode> grid)
    {
        Instance = this;
        this.grid = grid;

    }

    public Grid<GridObjectPathNode> GetGridObject()
    {
        return grid;
    }

    public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition)
    {
        grid.GetXY(startWorldPosition, out int startX, out int startY);
        grid.GetXY(endWorldPosition, out int endX, out int endY);

        List<GridObjectPathNode> path = FindPath(startX, startY, endX, endY);
        if(path == null)
        {
            return null;
        }
        else
        {
            List<Vector3> vectorPath = new List<Vector3>();
            int c = 0;
            foreach(GridObjectPathNode pathNode in path)
            {
                c++;
                vectorPath.Add(new Vector3(pathNode.x, pathNode.y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * .5f);
                Debug.LogWarning("PathNode vector " + c + " : " + new Vector3(pathNode.x, pathNode.y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * .5f);
            }
            int count = 0;
            foreach(Vector3 pathVector in vectorPath)
            {
                count++;
                Debug.LogWarning("pathfindin vector " +count+" : " + pathVector);
            }
            return vectorPath;
        }
    }

    public List<GridObjectPathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        GridObjectPathNode startNode = grid.GetGridObject(startX, startY);
        GridObjectPathNode endNode = grid.GetGridObject(endX, endY);

        openList = new List<GridObjectPathNode> {startNode };
        closedList = new List<GridObjectPathNode>();

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                GridObjectPathNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        while(openList.Count > 0)
        {
            GridObjectPathNode currentNode = GetLowesetFCostNode(openList);
            if(currentNode == endNode)
            {
                // Reached final node
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach( GridObjectPathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if (closedList.Contains(neighbourNode)) continue;
                if (!neighbourNode.isWalkable)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if(tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode,endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }


        // Out of nodes on the openList
        return null;
    }

    private List<GridObjectPathNode> GetNeighbourList(GridObjectPathNode currentNode)
    {
        List<GridObjectPathNode> neighbourList = new List<GridObjectPathNode>();

        // Left Neighbours
        if (currentNode.x - 1 >= 0)
        {
            // Left
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
            // Left Down
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
            // Left UP
            if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
        }
        // Right Neigbours
        if (currentNode.x + 1 < grid.GetWidth())
        {
            // Right
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
            // Right Lower
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
            // Right Up
            if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
        }
        // Middle Neighbours
        // Down
        if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));
        // Up
        if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighbourList;

    }

    public GridObjectPathNode GetNode(int x,int y)
    {
        return grid.GetGridObject(x, y);
    }

    private List<GridObjectPathNode> CalculatePath(GridObjectPathNode endNode)
    {
        List<GridObjectPathNode> path = new List<GridObjectPathNode>();
        path.Add(endNode);
        GridObjectPathNode currentNode = endNode;
        while (currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    private int CalculateDistanceCost(GridObjectPathNode a, GridObjectPathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private GridObjectPathNode GetLowesetFCostNode(List<GridObjectPathNode> pathNodeList)
    {
        GridObjectPathNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if(pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }

}
