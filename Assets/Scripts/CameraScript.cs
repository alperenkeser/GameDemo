using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public class CameraScript : MonoBehaviour
{
    Transform camT;

    Vector3 startPosition;
    Vector3 newPosition;

    Vector3 dragStartPosition;
    Vector3 dragCurrentPosition;

    float newZoom;

    float camMovementSpeed;
    public float normalSpeed;
    public float fastSpeed;
    public float movementTime;

    public float zoomAmount;

    //[SerializeField] GridBuildingSystem gridBuildSystem;
    [SerializeField] SelectingObjectController selectingObjectController;

    int gridHeight;
    int gridWidth;
    Vector3 gridOriginPos;
    float gridCellSize;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(waitCreatedGrid());

    }

    IEnumerator waitCreatedGrid()
    {
        yield return new WaitUntil(() => selectingObjectController.GetGrid() != null);

        gridHeight = selectingObjectController.GetGrid().GetHeight();
        gridWidth = selectingObjectController.GetGrid().GetWidth();
        gridOriginPos = selectingObjectController.GetGrid().GetOriginPosition();
        gridCellSize = selectingObjectController.GetGrid().GetCellSize();

        camT = this.transform;

        startPosition = new Vector3(Mathf.FloorToInt((gridOriginPos.x + (gridWidth * gridCellSize)) / 2),
                                    Mathf.FloorToInt((gridOriginPos.y + (gridHeight * gridCellSize)) / 2),
                                    camT.position.z);

        camT.position = startPosition;
        newPosition = startPosition;
        newZoom = camT.GetComponent<Camera>().orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseInput();
        HandleKeyboardInput();
        HandleMovement();
    }

    public void HandleMouseInput()
    {
        Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
        if (Input.GetMouseButtonDown(1))
        {
            dragStartPosition = mouseWorldPosition;
        }
        if (Input.GetMouseButton(1))
        {
            dragCurrentPosition = mouseWorldPosition;

            newPosition = transform.position + dragStartPosition - dragCurrentPosition;
        }
        if(Input.mouseScrollDelta.y != 0)
        {
            newZoom -= Input.mouseScrollDelta.y * zoomAmount;
        }
    }

    public void HandleKeyboardInput()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            camMovementSpeed = fastSpeed;
        }
        else
        {
            camMovementSpeed = normalSpeed;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition += transform.right * -camMovementSpeed;
        }
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            newPosition += transform.up * camMovementSpeed;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            newPosition += transform.right * camMovementSpeed;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            newPosition += transform.up * -camMovementSpeed;
        }

        //reset Camera position
        if (Input.GetKeyDown(KeyCode.R))
        {
            camT.position = startPosition;

            newPosition = startPosition;
        }


    }

    public void HandleMovement()
    {
        camT.position = Vector3.Lerp(camT.position, newPosition, Time.deltaTime * movementTime);
        camT.GetComponent<Camera>().orthographicSize = Mathf.Lerp(camT.GetComponent<Camera>().orthographicSize, newZoom, Time.deltaTime * movementTime);
    }
}
