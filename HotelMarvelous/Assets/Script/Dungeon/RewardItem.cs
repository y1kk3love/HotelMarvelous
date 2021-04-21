using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardItem : MonoBehaviour
{
    private GameObject _child;

    [SerializeField]
    private byte itemcode = 1;

    void Start()
    {
        _child = transform.GetChild(0).gameObject;
    }

    void Update()
    {
        if(itemcode == 0)
        {
            _child.GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            _child.GetComponent<Renderer>().material.color = Color.blue;
        }
    }

    public byte GetRewardItemcode()
    {
        return itemcode;
    }

    public void SetRewardItemcode(byte _code)
    {
        itemcode = _code;
    }
}