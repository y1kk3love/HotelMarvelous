using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Singleton;

public class ResourceManager : MonoSingleton<ResourceManager>
{
    private Dictionary<DIALOGZONE, List<DialogData>> dialogData = new Dictionary<DIALOGZONE, List<DialogData>>();

    private List<string[]> objectDataList = new List<string[]>();
    private List<DialogData> dialogDataList = new List<DialogData>();

    public void LoadResources()
    {
        LoadDialogData();
        LoadItemResources();
    }

    private void LoadDialogData()
    {
        StreamReader streader = new StreamReader(Application.dataPath + "/StreamingAssets/CSV/DialogData.csv");

        int counter = 0;
        DIALOGZONE pointindex = 0;

        while (!streader.EndOfStream)
        {
            DialogData dialog = new DialogData();

            string line = streader.ReadLine();

            string[] data = line.Split(',');
            string[] text = data[(int)DIALOGDATA.TEXT].Split('&');

            dialog.dialogTextArr = text;

            if (counter != 0)
            {
                if(data[(int)DIALOGDATA.POINTINDEX] != "")
                {
                    pointindex = (DIALOGZONE)int.Parse(data[(int)DIALOGDATA.POINTINDEX]);
                }

                if(data[(int)DIALOGDATA.TEXTEVENT] != "")
                {
                    dialog.dialogEvent = int.Parse(data[(int)DIALOGDATA.TEXTEVENT]);
                }

                if (dialogData.TryGetValue(pointindex, out List<DialogData> _value))
                {
                    dialogData[pointindex].Add(dialog);
                }
                else
                {
                    dialogDataList = new List<DialogData>();
                    dialogDataList.Add(dialog);

                    dialogData.Add(pointindex, dialogDataList);
                }
            }
            
            counter++;
        }
    }

    public string[] GetDialog(DIALOGZONE _point, int _index)
    {
        return dialogData[_point][_index].dialogTextArr;
    }

    public int GetDialogEvent(DIALOGZONE _point, int _index)
    {
        return dialogData[_point][_index].dialogEvent;
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

    public Sprite GetDispoItemResource(byte _itemindex)
    {
        string _path = objectDataList[_itemindex][(int)OBJECTDATA.DISPOITEM];

        Sprite sprite = Resources.Load<Sprite>("Prefab/DispoItem/" + _path);

        return sprite;
    }

    public ItemResource GetItemResource(byte _itemindex)
    {
        ItemResource item = new ItemResource();

        string _path = objectDataList[_itemindex][(int)OBJECTDATA.ITEMNAME];

        item.sprite = Resources.Load<Sprite>("Prefab/ActiveItem/Image/" + _path);
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

public class DialogData
{
    public string[] dialogTextArr;
    public int dialogEvent = -1;
}
