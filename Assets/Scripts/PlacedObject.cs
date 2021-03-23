using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedObject : MonoBehaviour 
{
    private ProductionObjects productionObject;
    private Vector2Int origin;
    private ProductionObjects.Dir dir;
    private ObjectTypes.objectType type;

    public static PlacedObject Create(Vector3 worldPos, Vector2Int origin, ProductionObjects.Dir dir, ProductionObjects productionObject, ObjectTypes.objectType type, float cellSize)
    {
        Transform placedObjectTransform = Instantiate(productionObject.prefab.transform,worldPos, Quaternion.Euler(0, 0, -productionObject.GetRotationAngle(dir)));

        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();

        Transform spriteTransform = placedObjectTransform.Find("New Sprite");

        Transform textMeshTransform = placedObjectTransform.Find("TextMesh");

        placedObjectTransform.localScale *= cellSize;

        placedObject.productionObject = productionObject;

        placedObjectTransform.name = productionObject.GetProducitonObjcetName();

        placedObject.origin = origin;
        placedObject.dir = dir;
        placedObject.type = type;


        return placedObject;
    }

    public Vector2Int GetOrigin()
    {
        return origin;
    }

    public void SetOrigin(Vector2Int newOrigin)
    {
        this.origin = newOrigin;
    }
    public List<Vector2Int> GetGridPositionList()
    {
        return productionObject.GetGridPositionList(origin, dir);
    }

    public List<Vector2Int> GetGridSpawnPosList()
    {
        return productionObject.GetGridSpawnPosList(origin, dir);
    }

    public int GetObjectWidth()
    {
        return productionObject.width;
    }
    public int GetObjectHeight()
    {
        return productionObject.height;
    }

    public Sprite GetInformationSprite()
    {
        return productionObject.informationSprite;
    }

    public Sprite GetProductionSpirte()
    {
        return productionObject.productionSprite;
    }

    public bool GetIsProductiable()
    {
        return productionObject.isProductiable;
    }

    public ObjectTypes.objectType GetGridObjectType()
    {
        return type;
    }

    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }

}
