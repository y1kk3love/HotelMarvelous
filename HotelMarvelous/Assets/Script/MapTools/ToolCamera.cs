using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolCamera : MonoBehaviour
{
    private Camera camera = null;

    GameObject obRoomPick;

    private float cameraWheelSpeed = 10.0f;
    private float minCamZoom = 20.0f;
    private float maxCamZoom = 1300.0f;

    private Vector2? dragGridStartPos = null;
    private Vector2 dragGridCurPos;
    private Vector2 dragGridCamPos;

    void Start()
    {
        camera = GetComponent<Camera>();

        obRoomPick = Resources.Load("MapTools/Prefab/RoomPick") as GameObject;
    }

    void Update()
    {
        CameraDragMove();
        CameraWheelZoom();

        FloorPick();
    }

    #region [CameraMove]

    //카메라 우클릭 드래그 이동
    private void CameraDragMove()
    {
        dragGridCurPos = Input.mousePosition;

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            dragGridStartPos = Input.mousePosition;
            dragGridCamPos = transform.position;
        }
        else if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            dragGridStartPos = null;
        }

        if (Input.GetKey(KeyCode.Mouse1) && dragGridStartPos != null)
        {
            Vector2 dir = (Vector2)dragGridStartPos - dragGridCurPos;

            transform.position = dragGridCamPos + dir * (camera.orthographicSize  / (minCamZoom + maxCamZoom) / 2);
        }
    }

    //카메라 휠 줌인 아웃
    private void CameraWheelZoom()           
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel") * cameraWheelSpeed * (camera.orthographicSize / -15.0f);

        if (camera.orthographicSize <= minCamZoom && scroll < 0)
        {
            camera.orthographicSize = minCamZoom;
        }
        else if(camera.orthographicSize >= maxCamZoom && scroll > 0)
        {
            camera.orthographicSize = maxCamZoom;
        }
        else
        {
            camera.orthographicSize += scroll;
        }
    }

    #endregion

    private void FloorPick()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Vector2 _Pos = hit.point;

                Debug.Log(_Pos);
            }
        }
    }

    private byte PosParse(int _pos)
    {
        return (byte)(_pos + 25);
    }
}

class RoomInfo
{

}
