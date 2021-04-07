using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ResourceManager : MonoBehaviour
{
    private ItemInfo iteminfo = new ItemInfo();

    private GameObject[] itemprefabArr;
    private Sprite[] itemspriteArr;
    private List<ItemInfo> itemList = new List<ItemInfo>();

    void Start()
    {
        GetItemReSources();
    }

    private void GetItemReSources()
    {
        string _path = Application.dataPath + "/Resources/Item/itemMagnification.txt";
        string[] textArr = File.ReadAllLines(_path);
        string[] _textarr = new string[1];
        GameObject tempobjet = null;
        int x = 0;

        itemspriteArr = Resources.LoadAll<Sprite>("Item/Image");
        itemprefabArr = Resources.LoadAll<GameObject>("Item/Prefab");

        foreach (string _text in textArr)
        {
            _textarr = _text.Split(' ');
        }

        for (int i = 0; i < itemprefabArr.Length + 1; i++)
        {
            if (x < itemspriteArr.Length - 1)
            {
                tempobjet = itemprefabArr[x];
            }

            if (tempobjet.name != itemspriteArr[i].name)
            {
                iteminfo.SetSprite(itemspriteArr[i]);
                iteminfo.SetObject(null);
                x++;
            }
            else
            {
                iteminfo.SetSprite(itemspriteArr[i]);
                iteminfo.SetObject(itemprefabArr[i]);
                x++;
            }

            iteminfo.SetMagnification(int.Parse(_textarr[i]));
            itemList.Add(iteminfo);
            Debug.Log(iteminfo);
        }
    }

    public GameObject GetItemPrefeb(int _itemcode)
    {
        return itemList[_itemcode].GetObject();
    }

    public Sprite GetItemSprite(int _itemcode)
    {
        return itemList[_itemcode].GetSprite();
    }

    public int GetItemMagnification(int _itemcode)
    {
        return itemList[_itemcode].GetMagnification();
    }
}

public class ItemInfo
{
    private int itemMag;
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

    public void SetMagnification(int _mag)
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
