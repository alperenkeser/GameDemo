using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class InfiniteScroll : MonoBehaviour , IDragHandler, IBeginDragHandler, IEndDragHandler
{
    #region variables
    public int additionalSize;

    private int contentPosY, scrollRectHeight, ItemHeight, halfScrollRectHeight;

    float dragDetectionAnchorPreviousY = 0;

    //[SerializeField]
    //private List<GameObject> poolList = new List<GameObject>();

    private ScrollRect scrollRect;
    public Scrollbar scrollbarVertical;

    
    [SerializeField] private GameObject ContentPrefab;
    [SerializeField] private GameObject ContentGO;
    [SerializeField] private ObjectPooling PooledItems;
    [SerializeField] private RectTransform DragObjectRT;
    [SerializeField] private RectTransform ViewPortRT;

    private RectTransform ContentRT;

    int dataHead = 0;
    int dataTail = 0;
    #endregion

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();

        ContentRT = ContentGO.GetComponent<RectTransform>();

    }

    private void Start()
    {
        scrollRect.onValueChanged.AddListener(OnDragDetectionPositionChange);

        ItemHeight = Mathf.RoundToInt(ContentPrefab.GetComponent<RectTransform>().rect.height);

        DragObjectRT.sizeDelta = new Vector2(DragObjectRT.sizeDelta.x,int.MaxValue);

        Debug.LogError("Visiable Items : " + VisibleItems + "\nOutOfViewItems : " + OutOfViewItems);

        StartCoroutine(WaitPoolItems());

    }

    IEnumerator WaitPoolItems()
    {
        Debug.LogWarning("StartTime : " + Time.time + PooledItems.isPoolDone);

        yield return new WaitUntil(() => PooledItems.isPoolDone == true);
    
        Debug.LogWarning("EndTime : " + Time.time + PooledItems.isPoolDone);


        for (int i = 0; i < VisibleItems + additionalSize; i++)
        {
            GameObject obj = PooledItems.ItemBorrow();
            obj.transform.SetParent(ContentGO.transform);
            obj.name = dataTail.ToString();
            obj.SetActive(true);
            dataTail++;
        }

    }

    private void Update()
    {

    }

    public void OnDragDetectionPositionChange(Vector2 dargNormalizePos)
    {
        float dragDelta = DragObjectRT.anchoredPosition.y - dragDetectionAnchorPreviousY;

        ContentRT.anchoredPosition = new Vector2(ContentRT.anchoredPosition.x,ContentRT.anchoredPosition.y + dragDelta);

        UpdateContentItems();

        dragDetectionAnchorPreviousY = DragObjectRT.anchoredPosition.y;
    }



    public void OnBeginDrag(PointerEventData eventData)
    {
        dragDetectionAnchorPreviousY = DragObjectRT.anchoredPosition.y;
    }

    public void OnDrag(PointerEventData eventData)
    {
    }


    public void OnEndDrag(PointerEventData eventData)
    {
    }

    private void UpdateContentItems()
    {
        Transform firstChildT = ContentRT.GetChild(0);
        Transform lastChildT = ContentRT.GetChild(ContentRT.childCount - 1);

        if (OutOfViewItems > additionalSize)
        {
            if(dataTail >= int.MaxValue)
            {
                return;
            }

            firstChildT.SetSiblingIndex(ContentRT.childCount - 1);
            firstChildT.name = dataTail.ToString();
            ContentRT.anchoredPosition = new Vector2(ContentRT.anchoredPosition.x,ContentRT.anchoredPosition.y - ItemHeight);
            dataHead++;
            dataTail++;
        }
        else if(OutOfViewItems < additionalSize)
        {
            if(dataHead <= 0)
            {
                return;
            }

            lastChildT.SetSiblingIndex(0);
            lastChildT.name = dataHead.ToString();
            dataHead--;
            dataTail--;
            ContentRT.anchoredPosition = new Vector2(ContentRT.anchoredPosition.x, ContentRT.anchoredPosition.y + ItemHeight);

        }
    }

    int VisibleItems { get { return Mathf.Max(Mathf.CeilToInt(ViewPortRT.rect.height / ItemHeight), 0); } }
    int OutOfViewItems { get { return Mathf.CeilToInt(ContentRT.anchoredPosition.y / ItemHeight); } }
}
