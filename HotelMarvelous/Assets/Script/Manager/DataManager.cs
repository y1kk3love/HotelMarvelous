using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Singleton;

public class DataManager : MonoSingleton<DataManager>
{
    private PlayerStatus playerStatus = new PlayerStatus();

    public PlayerStatus GetPlayerStatus()
    {
        return playerStatus;
    }

    public void SetPlayerStatus(PlayerStatus _stat)
    {
        playerStatus = _stat;
    }

    public void DataLoad()
    {
        //데이터 로드 관리
    }

    #region [Calculate]

    public float CalculateDamage()
    {
        int _percent = Random.Range(0, 100);
        float _damage = playerStatus.damage;

        if (_percent < playerStatus.missdam)
        {
            _damage -= 2;
        }
        else if (_percent < playerStatus.missdam + playerStatus.lightdam)
        {
            _damage--;
        }
        else if (_percent < playerStatus.missdam + playerStatus.lightdam + playerStatus.normaldam)
        {
            return _damage;
        }
        else if (_percent < playerStatus.missdam + playerStatus.lightdam + playerStatus.normaldam + playerStatus.harddam)
        {
            _damage++;
        }
        else
        {
            _damage += 2;
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
