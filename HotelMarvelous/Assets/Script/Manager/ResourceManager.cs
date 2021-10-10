using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Singleton;

public class ResourceManager : MonoSingleton<ResourceManager>
{
    private Dictionary<int, List<DialogData>> dialogData = new Dictionary<int, List<DialogData>>();

    private List<string[]> objectDataList = new List<string[]>();
    private List<DialogData> dialogDataList = new List<DialogData>();

    private Dictionary<int, List<DialogEventData>> dialogEventData = new Dictionary<int, List<DialogEventData>>();

    private List<DialogEventData> dialogEventList = new List<DialogEventData>();

    private GameObject[] dropItemArr;

    public void LoadResources()
    {
        LoadDialogData();
        LoadDialogEventData();
        LoadItemResources();
    }

    private void LoadDialogData()
    {
        StreamReader streader = new StreamReader(Application.dataPath + "/StreamingAssets/CSV/DialogData.csv");

        int counter = 0;
        int pointindex = 0;

        while (!streader.EndOfStream)
        {
            DialogData dialog = new DialogData();

            string line = streader.ReadLine();
            
            string[] data = line.Split(',');
            
            if(data.Length > 5)
            {
                string _text = data[(int)DIALOGDATA.TEXT];

                for (int i = 5; i < data.Length; i++)
                {
                    _text += string.Format(", {0}", data[i]);
                }

                data[(int)DIALOGDATA.TEXT] = _text;
            }

            string[] text = data[(int)DIALOGDATA.TEXT].Split('&');

            dialog.dialogTextArr = text;

            if (counter != 0)
            {
                if(data[(int)DIALOGDATA.POINTINDEX] != "")
                {
                    pointindex = int.Parse(data[(int)DIALOGDATA.POINTINDEX]);
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

    private void LoadDialogEventData()
    {
        StreamReader streader = new StreamReader(Application.dataPath + "/StreamingAssets/CSV/DialogEventData.csv");

        int counter = 0;
        int eventindex = 0;

        while (!streader.EndOfStream)
        {
            DialogEventData dialog = new DialogEventData();

            string line = streader.ReadLine();

            string[] data = line.Split(',');

            if (data.Length > 5)
            {
                string _text = data[(int)DIALOGEVENTDATA.EVENTDIALOG];

                for (int i = 5; i < data.Length; i++)
                {
                    _text += string.Format(", {0}", data[i]);
                }

                data[(int)DIALOGDATA.TEXT] = _text;
            }

            string[] pointdata = data[(int)DIALOGEVENTDATA.EVENTMOVETO].Split('_');
            string[] text = data[(int)DIALOGEVENTDATA.EVENTDIALOG].Split('&');

            if (counter != 0)
            {
                if(pointdata[0] != "")
                {
                    dialog.nextPoint = int.Parse(pointdata[0]);
                    dialog.nextDialogIndex = int.Parse(pointdata[1]);
                    
                }

                dialog.nextDialog = text;
                dialog.chice = data[(int)DIALOGEVENTDATA.EVENTCHICE];

                if (data[(int)DIALOGEVENTDATA.EVENTREWARD] != "")
                {
                    dialog.reward = int.Parse(data[(int)DIALOGEVENTDATA.EVENTREWARD]);
                }

                if (data[(int)DIALOGEVENTDATA.EVENTINDEX] != "")
                {
                    eventindex = int.Parse(data[(int)DIALOGDATA.POINTINDEX]);
                }

                if (dialogEventData.TryGetValue(eventindex, out List<DialogEventData> _value))
                {
                    dialogEventData[eventindex].Add(dialog);
                }
                else
                {
                    dialogEventList = new List<DialogEventData>();
                    dialogEventList.Add(dialog);

                    dialogEventData.Add(eventindex, dialogEventList);
                }
            }

            counter++;
        }
    }

    public List<DialogEventData> GetDialogEvent(int index)
    {
        return dialogEventData[index];
    }

    public string[] GetDialog(int _point, int _index)
    {
        return dialogData[_point][_index].dialogTextArr;
    }

    public int GetDialogEvent(int _point, int _index)
    {
        return dialogData[_point][_index].dialogEvent;
    }

    public GameObject GetDropItem(DISPOITEM id)
    {
        if(dropItemArr == null)
        {
            dropItemArr = Resources.LoadAll<GameObject>("Prefab/Item");        
        }

        return dropItemArr[(byte)id];
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

public class DialogEventData
{
    public int nextPoint = -1;
    public int nextDialogIndex = -1;
    public string[] nextDialog;

    public string chice = "";
    public int reward = 0;
}
