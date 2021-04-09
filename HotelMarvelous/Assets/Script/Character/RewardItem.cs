using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardItem : MonoBehaviour
{
    private GameObject _child;

    [SerializeField]
    private int itemcode = 1;

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

    public int GetRewardItemcode()
    {
        return itemcode;
    }

    public void SetRewardItemcode(int _code)
    {
        itemcode = _code;
    }
}