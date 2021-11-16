using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;

public class DataManager : MonoSingleton<DataManager>
{
    private PlayerStatus playerStatus = new PlayerStatus();

    private int[] favorabilityarr = new int[20];

    public PlayerStatus GetPlayerStatus()
    {
        playerStatus = new PlayerStatus();
        return playerStatus;
    }

    public void SetPlayerStatus(PlayerStatus _stat)
    {
        playerStatus = _stat;
    }

    public void SetNPCFavorability(byte id, int levels)
    {
        favorabilityarr[id] += levels;
    }

    public int GetNPCFavorability(byte id)
    {
        return favorabilityarr[id];
    }

    public void DataLoad()
    {
        //데이터 로드 관리
    }

    #region [Calculate]

    public float CalculateDamage()
    {
        byte[] damagePerArr = new byte[] { 15, 15, 40, 15, 15 };

        int _percent = Random.Range(0, 101);
        int _curpercent = 0;
        float _damage = playerStatus.damage;


        for(int i = 0; i < 5; i++)
        {
            _curpercent += damagePerArr[i];

            if (_percent <= _curpercent)
            {
                _damage -= i - 2;
            }
        }

        if (CalculateIsCritical())
        {
            _damage *= 1.8f;
        }

        Debug.Log(_damage);
        return _damage;
    }

    private bool CalculateIsCritical()
    {
        int curpercent = Random.Range(0, 1000);

        if (curpercent < playerStatus.criticalPercent * 10)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion
}
