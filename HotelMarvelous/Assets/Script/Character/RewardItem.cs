using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardItem : MonoBehaviour
{
    [SerializeField]
    private float rotatespeed = 0.35f;

    private GameObject _child;

    [SerializeField]
    private int itemcode = 1;

    void Start()
    {
        _child = transform.GetChild(0).gameObject;
    }

    void Update()
    {
        _child.transform.Rotate(0, rotatespeed, 0, Space.World);
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