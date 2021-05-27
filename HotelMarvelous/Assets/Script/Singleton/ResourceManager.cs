using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using game;

public class ResourceManager : MonoSingleton<ResourceManager>
{
    private GameObject[] monsterArr;

    private GameObject[] activeitemprefabArr;    
    private Sprite[] activeitemspriteArr;
    private List<ItemInfo> activeitemList = new List<ItemInfo>();

    void Start()
    {
        monsterArr = Resources.LoadAll<GameObject>("Prefab/Characters/Monsters");
    }

    public void GetItemReSources()
    {
        string _path = Application.dataPath + "/StreamAssets/itemMagnification.txt";
        string[] textArr = File.ReadAllLines(_path);
        string[] _textarr = new string[1];
        GameObject tempobjet = null;
        int x = 0;

        activeitemspriteArr = Resources.LoadAll<Sprite>("Prefab/ActiveItem/Image");
        activeitemprefabArr = Resources.LoadAll<GameObject>("Prefab/ActiveItem/Object");

        foreach (string _text in textArr)
        {
            _textarr = _text.Split(' ');
        }

        for (int i = 0; i < activeitemprefabArr.Length + 1; i++)
        {
            ItemInfo iteminfo = new ItemInfo();

            if (x < activeitemspriteArr.Length - 1)
            {
                tempobjet = activeitemprefabArr[x];
            }

            if (tempobjet.name != activeitemspriteArr[i].name)
            {
                iteminfo.SetSprite(activeitemspriteArr[i]);
                iteminfo.SetObject(null);
            }
            else
            {
                iteminfo.SetSprite(activeitemspriteArr[i]);
                iteminfo.SetObject(activeitemprefabArr[i]);
                x++;
            }

            iteminfo.SetMagnification(byte.Parse(_textarr[i]));
            activeitemList.Add(iteminfo);
        }
    }

    public GameObject LoadGameObject(string _path)
    {
        return Resources.Load<GameObject>(_path);
    }
    public GameObject GetItemPrefeb(byte _itemcode)
    {
        return activeitemList[_itemcode].GetObject();
    }

    public Sprite GetItemSprite(byte _itemcode)
    {
        return activeitemList[_itemcode].GetSprite();
    }

    public int GetItemMagnification(byte _itemcode)
    {
        return activeitemList[_itemcode].GetMagnification();
    }

    public GameObject GetMonsterArr(byte _index)
    {
        return monsterArr[_index];
    }
}

public class ItemInfo
{
    private byte itemMag;
    private GameObject itemprefab;
    private Sprite sprite;

    public void SetObject(GameObject _prefab)
    {
        itemprefab = _prefab;
    }

    public void SetSprite(Sprite _sprite)
    {
        sprite = _sprite;
    }

    public void SetMagnification(byte _mag)
    {
        itemMag = _mag;
    }

    public GameObject GetObject()
    {
        return itemprefab;
    }

    public Sprite GetSprite()
    {
        return sprite;
    }

    public int GetMagnification()
    {
        return itemMag;
    }
}
