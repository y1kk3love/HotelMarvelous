using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WideAreaSkill : MonoBehaviour
{
    private string targettag;

    private bool isexitzone = false;

    private float skilldelay = 0;
    private float skillendtime = 0;
    private float skilldamage = 0;

    void Update()
    {
        skilldelay += Time.deltaTime;

        if (skilldelay >= skillendtime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targettag))
        {
            isexitzone = false;

            if(targettag == "Monster" && targettag == "Boss")
            {
                StartCoroutine(FireTile(other, skilldamage, 1, targettag));
            }
            else if (targettag == "Player")
            {
                StartCoroutine(FireTile(other, skilldamage, 1, targettag));
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(targettag))
        {
            isexitzone = true;
        }
    }

    public void SetSkillPreset(string _tag, float _damage, float _endtime)
    {
        skilldamage = _damage;
        skillendtime = _endtime;
        targettag = _tag;
    }

    IEnumerator FireTile(Collider _other, float _damage, float _delay, string _targettag)
    {
        if(_other != null)
        {
            if(_targettag == "Monster")
            {
                _other.GetComponent<Monster>().MonGetDamage(_damage);
            }
            else if(_targettag == "Player")
            {
                _other.GetComponent<Player>().SetDamage(_damage);
            }
        }

        yield return new WaitForSeconds(_delay);

        if (!isexitzone)
        {
            StartCoroutine(FireTile(_other, _damage, _delay, _targettag));
        }      
    }
}
