using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Utils;

public class SelectingObjectController : MonoBehaviour 
{
    [SerializeField] private Grid<GridObjectPathNode> gridObjectPathNode;

    [SerializeField] private List<ProductionObjects> productionObjectsList;
    private ProductionObjects productionObject;
    private ProductionObjects soldierObject;

    private ProductionObjects.Dir dir = ProductionObjects.Dir.Down;

    [SerializeField] private InformationPanel informationPanel;

    [SerializeField] private MouseTextUI mouseTextUI;

    //[SerializeField] private GridView gridView;

    private Pathfinding pathfinding;

    private bool isSelected;

    private PlacedObject SelectedObject;

    private SoldierMovement soldierMovement;

    private void Awake()
    {
        int gridWidth = 10;
        int gridHeight = 10;
        float cellSize = 10f;
        gridObjectPathNode = new Grid<GridObjectPathNode>(gridWidth, gridHeight, cellSize, Vector3.zero, (Grid<GridObjectPathNode> grid, int x, int y) => new GridObjectPathNode(grid, x, y));

        productionObject = productionObjectsList[0];
    }

    void Start()
    {
        isSelected = false;

        pathfinding = new Pathfinding(gridObjectPathNode);

        //gridView.SetGrid(gridObjectPathNode);
    }
    
    public void Update()
    {
        Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();

        if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject()) // Select UI
        {
            //Debug.Log("-------Mouse-Clicked-------");
            if (EventSystem.current != null)
            {
                Debug.Log("Current : " + EventSystem.current);
                Debug.Log("IsPointerOverGameObject : " + EventSystem.current.IsPointerOverGameObject());


                if (EventSystem.current.currentSelectedGameObject != null)
                {
                    //Debug.Log("current Selected GameObject : " + EventSystem.current.currentSelectedGameObject);

                    if (EventSystem.current.currentSelectedGameObject.name != null)
                    {
                        if (EventSystem.current.currentSelectedGameObject.name == "BarracksButton")
                        {
                            productionObject = productionObjectsList[0];
                            UtilsClass.CreateWorldTextPopup(productionObject.nameString, mousePosition);
                            isSelected = true;
                        }
                        if (EventSystem.current.currentSelectedGameObject.name == "PowerPlantsButton")
                        {
                            productionObject = productionObjectsList[1];
                            UtilsClass.CreateWorldTextPopup(productionObject.nameString, mousePosition);
                            isSelected = true;
                        }
                        if(EventSystem.current.currentSelectedGameObject.name == "SoldierButton")
                        {
                            soldierObject = productionObjectsList[2];
                            InstatiateSoldier(mousePosition);
                        }



                        UtilsClass.CreateWorldTextPopup("Selected", mousePosition);
                        Debug.Log("currentSGO Name : " + EventSystem.current.currentSelectedGameObject.name);
                    }
                    // else Debug.Log("Name Null");
                }
                // else Debug.Log("current Selected GameObject Null");
            }
            // else Debug.Log("Current Null");

            //Debug.Log("-------Mouse-Clicked-------");
        }
        else if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) //Select GameObject
        {
            if (gridObjectPathNode.IsMouseInGrid(mousePosition))
            {
                if (isSelected)
                {
                    GridBuildingSystemFunc(mousePosition);
                    Debug.Log("Builded");
                }
                else
                {
                    gridObjectPathNode.GetXY(mousePosition, out int x, out int y);

                    Debug.Log("Is Grid GameObject Exist : " + gridObjectPathNode.GetGridObject(x, y).GetPlacedObject() != null);

                    SelectedObject = gridObjectPathNode.GetGridObject(x, y).GetPlacedObject();

                    if (SelectedObject != null)
                    {
                        Debug.LogWarning("NameProduction : " + SelectedObject.name +
                                         "Origin : " + SelectedObject.GetOrigin());

                        GridObjectInformation(mousePosition, SelectedObject);

                    }
                    else
                    {
                        SelectedObject = null;
                        informationPanel.isInformationAvailable(false);
                    }
                }

            }
        }

        if (Input.GetMouseButtonDown(1) && isSelected )
        {
            isSelected = false;
            UtilsClass.CreateWorldTextPopup("Deselected", mousePosition);
        }
        else if (Input.GetMouseButtonDown(1) && !isSelected && SelectedObject.name == "Soldier" && gridObjectPathNode.IsMouseInGrid(mousePosition))
        {
            gridObjectPathNode.GetXY(mousePosition,out int endX, out int endY);
            int startX = SelectedObject.GetOrigin().x;
            int startY = SelectedObject.GetOrigin().y;

            soldierMovement = SelectedObject.gameObject.GetComponent<SoldierMovement>();

            soldierMovement.SetSoldierObjectVariable(new Vector3(endX, endY),SelectedObject, gridObjectPathNode,pathfinding);

        }

        mouseTextUI.SetIsSelected(isSelected, productionObject.nameString);

    }

    public void GridBuildingSystemFunc(Vector3 mousePosition)
    {
        gridObjectPathNode.GetXY(mousePosition, out int x, out int y);

        List<Vector2Int> gridPositionList = productionObject.GetGridPositionList(new Vector2Int(x, y), dir);

        ObjectTypes.objectType gridObjectType = ObjectTypes.objectType.None;

        if (productionObject.GetProducitonObjcetName() == "Barrack") gridObjectType = ObjectTypes.objectType.Barrack;
        if (productionObject.GetProducitonObjcetName() == "PowerPlant") gridObjectType = ObjectTypes.objectType.PowerPlant;
        if (productionObject.GetProducitonObjcetName() == "Soldier") gridObjectType = ObjectTypes.objectType.Soldier;

        if (gridObjectPathNode.GetGridObject(x, y).GetPlacedObject() != null) Debug.LogWarning("Name : " + gridObjectPathNode.GetGridObject(x, y).GetPlacedObject().name);
        //Debug.LogWarning("Name : "+ productionObject.GetProducitonObjcetName() + ", " + productionObject.name);

        //Test can build
        bool canBuild = true;
        foreach (Vector2Int gridPosition in gridPositionList)
        {
            if (!gridObjectPathNode.GetGridObject(gridPosition.x, gridPosition.y).CanBuild())
            {
                // Cannot build here 
                canBuild = false;
                break;
            }
        }


        GridObjectPathNode gridObjectPN = gridObjectPathNode.GetGridObject(x, y);
        if (canBuild)
        {
            Vector2Int rotationOffset = productionObject.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = gridObjectPathNode.GetWorldPosition(x, y) + new Vector3(rotationOffset.x, rotationOffset.y, 0) * gridObjectPathNode.GetCellSize();


            PlacedObject placedObject = PlacedObject.Create(placedObjectWorldPosition, new Vector2Int(x, y), dir, productionObject, gridObjectType, gridObjectPathNode.GetCellSize());
            //Transform builtTransform = Instantiate(productionObject.prefab.transform, placedObjectWorldPosition, Quaternion.Euler(0, 0, -productionObject.GetRotationAngle(dir)));

            foreach (Vector2Int gridPosition in gridPositionList)
            {
                gridObjectPathNode.GetGridObject(gridPosition.x, gridPosition.y).SetPlacedObject(placedObject);
            }
            //gridObject.SetPlacedObject(placedObject);
            isSelected = false;
        }
        else
        {
            UtilsClass.CreateWorldTextPopup("Cannot build here! ", mousePosition);
        }
    }

    public void InstatiateSoldier(Vector3 mousePosition)
    {
        Vector2Int SelectedObjectOrigin = SelectedObject.GetOrigin();

        int x = 0;
        int y = 0;


        List<Vector2Int> gridSpawnPosList = SelectedObject.GetGridSpawnPosList();

        bool canSpawn = false;

        float oldDistance = int.MaxValue;

        foreach (Vector2Int gridSpawnPos in gridSpawnPosList)
        {
            Debug.LogWarning("isGridObjNull (x:" + gridSpawnPos.x + " y:" + gridSpawnPos.y + "):"+ gridObjectPathNode.GetGridObject(gridSpawnPos.x, gridSpawnPos.y) + " => " + (gridObjectPathNode.GetGridObject(gridSpawnPos.x, gridSpawnPos.y) != null));
            if (gridObjectPathNode.GetGridObject(gridSpawnPos.x, gridSpawnPos.y) != null && gridObjectPathNode.GetGridObject(gridSpawnPos.x, gridSpawnPos.y).CanBuild())
            {
                Debug.LogWarning("canBuild (x:" + gridSpawnPos.x + " y:" + gridSpawnPos.y + "): " + gridObjectPathNode.GetGridObject(gridSpawnPos.x, gridSpawnPos.y).CanBuild());

                float newDistance = UtilsClass.MeasureDist(SelectedObjectOrigin, gridSpawnPos);

                if(newDistance <= oldDistance)
                {
                    x = gridSpawnPos.x;
                    y = gridSpawnPos.y;
                    oldDistance = newDistance;
                }
                Debug.LogWarning("gridSpawnPos X : " + gridSpawnPos.x);

                canSpawn = true;
            }
        }

        List<Vector2Int> gridSoldierPosList = soldierObject.GetGridPositionList(new Vector2Int(x, y),dir);

        if (canSpawn)
        {
            Vector2Int rotationOffset = soldierObject.GetRotationOffset(dir);
            Vector3 placedObjectWorldPosition = gridObjectPathNode.GetWorldPosition(x,y) + new Vector3(rotationOffset.x, rotationOffset.y, 0) * gridObjectPathNode.GetCellSize();


            PlacedObject placedObject = PlacedObject.Create(placedObjectWorldPosition, new Vector2Int(x, y), dir, soldierObject, ObjectTypes.objectType.Soldier, gridObjectPathNode.GetCellSize());
            //Transform builtTransform = Instantiate(productionObject.prefab.transform, placedObjectWorldPosition, Quaternion.Euler(0, 0, -productionObject.GetRotationAngle(dir)));

            foreach (Vector2Int gridSoldierPos in gridSoldierPosList)
            {
                gridObjectPathNode.GetGridObject(gridSoldierPos.x, gridSoldierPos.y).SetPlacedObject(placedObject);
            }
            //gridObject.SetPlacedObject(placedObject);
        }
        else
        {
            UtilsClass.CreateWorldTextPopup("Cannot Spawn the grid! ", new Vector3(x, y));
        }
    }

    public Grid<GridObjectPathNode> GetGrid()
    {
        return gridObjectPathNode;
    }

    public void GridObjectInformation(Vector3 mousePosition,PlacedObject ObjectInfo)
    {

        #region information Variable
        string gridObjectName1 = ObjectInfo.name;
        string gridObjectName = ObjectInfo.name;
        Sprite informationSprite = ObjectInfo.GetInformationSprite();
        bool isProductiable = ObjectInfo.GetIsProductiable();
        Sprite productionSprite = null;
        if (isProductiable) productionSprite = ObjectInfo.GetProductionSpirte();
        #endregion

        //Giving Varialbe to information panel
        informationPanel.isInformationAvailable(true);
        informationPanel.SetInformationText(gridObjectName);
        informationPanel.SetInformationSprite(informationSprite);

        //Giving Varialbe to information pruduction panel
        informationPanel.isProductionAvailable(isProductiable);
        if (isProductiable)
        {
            informationPanel.SetProductionSprite(productionSprite);
        }

    }

}
