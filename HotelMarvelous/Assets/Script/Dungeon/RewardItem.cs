using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardItem : MonoBehaviour
{
    public CONSUMITEM id;

    public GameObject[] item;

    private float speed = 100;

    void Start()
    {
        ChangeItem(id);
    }

    void Update()
    {
        item[2].transform.Rotate(new Vector3(0, Time.deltaTime * speed, 0));
    }

    public void ChangeItem(CONSUMITEM _id)
    {
        id = _id;
        Texture2D _image = ResourceManager.Instance.GetItemResource((byte)id).image;

        item[0].transform.GetComponent<MeshRenderer>().material.mainTexture = _image;
        item[1].transform.GetComponent<MeshRenderer>().material.mainTexture = _image;
    }
}
