using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WideAreaSkill : MonoBehaviour
{
    private string targettag;

    private bool isexitzone = false;

    private float skilldelay = 0;
    private float skillendtime = 0;
    private float zscale = 0.5f;

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

            if(targettag == "Monster")
            {
                StartCoroutine(FireTile(other, 20, 1, targettag));
            }
            else if (targettag == "Player")
            {
                StartCoroutine(FireTile(other, 9, 1, targettag));
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

    public void SetSkillPreset(string _tag, float _endtime)
    {
        skillendtime = _endtime;
        targettag = _tag;
    }

    public void SetSkillRange(float _zscale)
    {
        zscale = _zscale;
        transform.localScale = new Vector3(0.3f, 1, zscale);
    }

    IEnumerator FireTile(Collider _other, int _damage, float _delay, string _targettag)
    {
        if(_other != null && !isexitzone)
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

        StartCoroutine(FireTile(_other, _damage, _delay, _targettag));
    }
}
