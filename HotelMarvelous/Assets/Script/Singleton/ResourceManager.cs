using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Singleton;

public class ResourceManager : MonoSingleton<ResourceManager>
{
    private Dictionary<DIALOGZONE, List<string[]>> dialogData = new Dictionary<DIALOGZONE, List<string[]>>();

    private List<string[]> objectDataList = new List<string[]>();
    private List<string[]> dialogDataList = new List<string[]>();

    public void LoadResources()
    {
        LoadDialogData();
        LoadItemResources();
    }

    private void LoadDialogData()
    {
        StreamReader streader = new StreamReader(Application.dataPath + "/StreamingAssets/CSV/DialogData.csv");

        int counter = 0;

        while (!streader.EndOfStream)
        {
            string line = streader.ReadLine();

            string[] data = line.Split(',');
            string[] text = data[(int)DIALOGDATA.TEXT].Split('&');

            if(counter != 0)
            {
                DIALOGZONE pointindex = (DIALOGZONE)int.Parse(data[(int)DIALOGDATA.POINTINDEX]);

                if(dialogData.TryGetValue(pointindex, out List<string[]> _value))
                {
                    dialogData[pointindex].Add(text);
                }
                else
                {
                    dialogDataList = new List<string[]>();
                    dialogDataList.Add(text);

                    dialogData.Add(pointindex, dialogDataList);
                }
            }
            
            counter++;
        }
    }

    private void LoadItemResources()
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
