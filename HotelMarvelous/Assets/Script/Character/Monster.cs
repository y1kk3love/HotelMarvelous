using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public int monsterid;

    private int touchdamage;
    private int monsterhp;

    void Start()
    {
        switch (monsterid)
        {
            case 1:
                touchdamage = 10;
                monsterhp = 10;
                break;
        }
    }

    public void MonGetDamage(int _damage)
    {
        monsterhp -= _damage;
    }
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();
            player.GetDamage(touchdamage);
            Debug.Log(player.CheckHp());
        }
    }
}
