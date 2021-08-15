using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Singleton;

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

    public ItemResource GetItemResource(byte _itemindex)
    {
        ItemResource item = new ItemResource();

        string _path = objectDataList[_itemindex][(int)OBJECTDATA.ITEMNAME];

        item.sprite = Resources.Load("Prefab/ActiveItem/Image/" + _path) as Sprite;
        item.skillprefab = Resources.Load("Prefab/ActiveItem/Object/" + _path) as GameObject;
        item.max = byte.Parse(objectDataList[_itemindex][(int)OBJECTDATA.MAXSTACK]);

        return item;
    }
}

public class ItemResource
{
    public Sprite sprite;
    public GameObject skillprefab;
    public byte max;
}
