using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using game;

public class ResourceManager : MonoSingleton<ResourceManager>
{
    private List<string[]> objectDataList = new List<string[]>();

    public void LoadItemResources()
    {
        StreamReader streader = new StreamReader(Application.dataPath + "/StreamingAssets/CSV/ObjectData.csv");

        while (!streader.EndOfStream)
        {
            string line = streader.ReadLine();

            string[] data = line.Split(',');

            objectDataList.Add(data);
        }
    }

    public void GetItemResource(byte _itemindex)
    {
        string _path = objectDataList[_itemindex][(int)OBJECTDATA.ITEMNAME];

        Sprite _sprite = Resources.Load("Prefab/ActiveItem/Image/" + _path) as Sprite;
        GameObject _object = Resources.Load("Prefab/ActiveItem/Image/" + _path) as GameObject;
        byte _max = byte.Parse(objectDataList[_itemindex][(int)OBJECTDATA.MAXSTACK]);

        GameObject _player = GameObject.Find("Player");

        if(_player != null)
        {
            _player.GetComponent<Player>()
        }
    }
}
