using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ROOMTYPE
{
    GUEST,
    HALLWAY,

}

public class MapManager : MonoBehaviour
{
    private int[,] mapboard = new int[0,0];

    private void Start()
    {
        
    }
}
