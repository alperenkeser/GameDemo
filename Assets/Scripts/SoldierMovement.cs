using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class SoldierMovement : MonoBehaviour
{
    private const float speed = 40f;

    private int currentPathIndex;
    private float gridCellSize;

    private List<Vector3> pathVectorList;
    private List<Vector3> pathGridList;

    private Vector3 targetPosition;

    private PlacedObject soldierObject;
    private Grid<GridObjectPathNode> grid;
    private Pathfinding pathfinding;

    bool isTherePath;

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        if (pathVectorList != null && pathVectorList.Count>0 && isTherePath)
        {

            if(pathGridList[currentPathIndex +1 ] != null)
            {
                if (!grid.GetGridObject((int)pathGridList[currentPathIndex + 1].x, (int)pathGridList[currentPathIndex + 1].y).isWalkable)
                {
                    StopMoving();
                    SetSoldierObjectVariable(pathGridList[pathGridList.Count - 1], this.GetComponent<PlacedObject>(), this.grid,pathfinding);
                }
            }

            if (currentPathIndex < pathVectorList.Count)
            {
                Vector3 targetPosition = pathVectorList[currentPathIndex];

                if (Vector3.Distance(transform.position, targetPosition) > 0.5f)
                {
                    Vector3 moveDir = (targetPosition - transform.position).normalized;

                    transform.position = transform.position + moveDir * speed * Time.deltaTime;

                    //transform.position = Vector3.Lerp(transform.position,targetPosition,15f * Time.deltaTime);

                    for (int i = currentPathIndex; i < pathVectorList.Count - 1; i++)
                    {
                        Debug.DrawLine(pathVectorList[i] + Vector3.one * gridCellSize * .5f, pathVectorList[i + 1] + Vector3.one * gridCellSize * .5f, Color.green);
                    }
                }
                else
                {
                    currentPathIndex++;

                    if (currentPathIndex >= pathVectorList.Count)
                    {
                        StopMoving();
                    }
                    grid.GetGridObject((int)pathGridList[currentPathIndex - 1].x, (int)pathGridList[currentPathIndex - 1].y).SetPlacedObject(null);
                    soldierObject.SetOrigin(new Vector2Int((int)pathGridList[currentPathIndex].x, (int)pathGridList[currentPathIndex].y));
                    grid.GetGridObject((int)pathGridList[currentPathIndex].x, (int)pathGridList[currentPathIndex].y).SetPlacedObject(soldierObject);


                }
            }

            
        }

    }

    private void StopMoving()
    {
        pathVectorList = null;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SetSoldierObjectVariable(Vector3 targetPosition,PlacedObject soldierObject,Grid<GridObjectPathNode> grid,Pathfinding pathfinding)
    {
        currentPathIndex = 0;
        isTherePath = true;

        this.soldierObject = soldierObject;
        this.targetPosition = targetPosition;
        this.grid = grid;
        this.pathfinding = pathfinding;

        SoldierPathfinding();

        Debug.LogWarning("Target Pos : " + targetPosition);

        if (pathVectorList != null && pathVectorList.Count > 1)
        {
            pathVectorList.RemoveAt(0);
        }
    }

    public void SoldierPathfinding()
    {
        int startX = soldierObject.GetOrigin().x;
        int startY = soldierObject.GetOrigin().y;


        pathVectorList = new List<Vector3>();
        pathGridList = new List<Vector3>();
        Debug.LogWarning("targetPos : " + targetPosition);
        List<GridObjectPathNode> path = pathfinding.FindPath(startX, startY, (int)targetPosition.x, (int)targetPosition.y);

        gridCellSize = grid.GetCellSize();
        
        Vector3 pathVector;
        Vector3 pathGrid;


        if (path != null)
        {
            for (int i = 0; i < path.Count; i++)
            {
                pathGrid = new Vector3(path[i].x,path[i].y);
                pathVector = pathGrid * gridCellSize;

                pathGridList.Add(pathGrid);
                pathVectorList.Add(pathVector);

                Debug.LogWarning("path " + i + " : " + path[i].x + "," + path[i].y);

            }
        }
        else
        {
            Debug.LogWarning("path is null");
        }

        if (pathGridList != null && pathGridList.Count > 0)
        {
            for (int i = 0; i < pathGridList.Count; i++)
            {
                Debug.LogWarning("path Grid List " + (i+1) + " : " + pathGridList[i]);
            }
        }
        if (pathVectorList != null && pathVectorList.Count < 1)
        {
            isTherePath = false;
            UtilsClass.CreateWorldTextPopup("There is no path");
        }
    }

}
