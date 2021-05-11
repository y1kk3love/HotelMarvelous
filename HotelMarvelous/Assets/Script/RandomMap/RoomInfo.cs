using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    private string roomtype;

    void Start()
    {
        roomtype = gameObject.name;

        if(roomtype.Substring(2,1) == "V")
        {

        }
        else if (roomtype.Substring(2, 1) == "H")
        {

        }
    }
}
