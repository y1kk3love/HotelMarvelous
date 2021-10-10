using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardItem : MonoBehaviour
{
    public CONSUMITEM id;

    public GameObject pos;

    void Start()
    {
        ChangeItem(id);
    }

    public void ChangeItem(CONSUMITEM _id)
    {
        switch (_id)
        {
            case CONSUMITEM.CROWN:
                pos.transform.GetComponent<MeshRenderer>().material.color = new Vector4(1, 0, 0, 1);
                break;
            case CONSUMITEM.SLOTMACHINE:
                pos.transform.GetComponent<MeshRenderer>().material.color = new Vector4(0, 1, 0, 1);
                break;
        }
    }
}
