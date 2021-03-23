using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    [SerializeField]
    private GameObject Prefab;

    public GameObject parentObj;

    public int PoolSize;

    private int head = 0;

    public bool isPoolDone;
    //[HideInInspector]
    //public List<GameObject> ListObjects = new List<GameObject>();

    private void Start()
    {
        isPoolDone = false;
        for (int i = 0; i < PoolSize; i++)
        {
            GameObject newObj = Instantiate(Prefab, parentObj.transform);
            newObj.SetActive(false);
            //ListObjects.Add(newObj);
        }
        isPoolDone = true;
    }

    public GameObject ItemBorrow()
    {
        if ( head >= PoolSize)
        {
            return null;
        }
        head++;
        return parentObj.transform.GetChild(0).gameObject;

    }

    public void ItemReturn(GameObject obj)
    {
        if(head <= 0){
            return;
        }

        head--;
        obj.SetActive(false);
        obj.transform.SetParent(parentObj.transform);
    }

}
