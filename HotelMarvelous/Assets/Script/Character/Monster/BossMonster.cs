using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonster : MonoBehaviour
{
    private Player player;
    private BossMove move;

    private Animator anim;

    private GameObject[] effects;
    public GameObject[] summon;
    private GameObject obplayer;
    public GameObject bossUI;
    public GameObject dialogzone;
    public GameObject weapon;
    public GameObject wateringcan;

    private Vector3 direction;

    public bool isintrofin = false;
    public bool isweapon = false;
    public bool ispattern = false;

    private float distance = 100;

    private float timer = 5f;

    public float maxHP = 70;
    public float curHP = 70;

    void Start()
    {
        anim = transform.GetComponent<Animator>();
        move = transform.GetComponent<BossMove>();
        effects = Resources.LoadAll<GameObject>("Effect/");
    }

    void Update()
    {
        DistanceCheck();
        FloraProcess();
    }

    private void DistanceCheck()
    {
        if (obplayer == null)
        {
            obplayer = GameObject.Find("Player");

            if (obplayer != null)
            {
                player = obplayer.GetComponent<Player>();
            }
        }
        else
        {
            Vector3 ppos = obplayer.transform.position;
            Vector3 bpos = transform.position;
            ppos.y = 0;
            bpos.y = 0;

            distance = Vector3.Distance(ppos, bpos);
            direction = (ppos - bpos).normalized;
        }
    }

    private void FloraProcess()
    {
        if(!isintrofin)
        {
            move.stop = true;

            if (distance <= 5)
            {
                anim.SetBool("Intro", true);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction * -1), Time.deltaTime * 3);
            }
        }
        else
        {
            timer -= Time.deltaTime;

            if(timer <= 0)
            {
                switch (Random.Range(0, 2))
                {
                    case 0:
                        StartCoroutine(ConductSummonAttack());
                        break;
                    case 1:
                        StartCoroutine(FlowAttack());
                        break;
                }
            }
        }
    }

    public void FinishIntro()
    {
        isintrofin = true;
        
        wateringcan.SetActive(false);
        dialogzone.SetActive(false);

        anim.SetBool("Intro", false);
        anim.SetTrigger("GetWeapon");
        isweapon = true;

        GameObject bossui = Instantiate(bossUI);
        bossui.name = "BossUI";
    }

    public void MonGetDamage(float _damage)
    {
        if (move.stop)
        {
            return;
        }

        curHP -= _damage;

        if (curHP <= 0)
        {
            weapon.SetActive(false);
            move.stop = true;

            anim.SetTrigger("Dead");

            Instantiate(effects[19], weapon.transform.position, Quaternion.identity);
            GameObject manager = GameObject.Find("DungeonManager");
            manager.GetComponent<DungeonEvent>().isBossDead = true;
            manager.GetComponent<DungeonMaker>().MonsterDead();

            Destroy(transform.gameObject, 2.5f);
        }
    }

    private IEnumerator FlowAttack()
    {
        timer = 500;

        if (isweapon)
        {
            anim.SetTrigger("PutWeapon");

            yield return new WaitForSeconds(2f);

            move.stop = true;
        }

        anim.SetBool("Move", false);
        anim.SetTrigger("FlowUp");

        GameObject efta = Instantiate(effects[20], new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);
        GameObject eftb = Instantiate(effects[21], new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);
        efta.transform.SetParent(transform);
        eftb.transform.SetParent(transform);

        GameObject _summon = Instantiate(effects[22], new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);

        yield return new WaitForSeconds(4f);

        anim.SetTrigger("FlowDown");

        yield return new WaitForSeconds(2f);

        if (!isweapon)
        {
            anim.SetTrigger("GetWeapon");

            yield return new WaitForSeconds(2f);
        }

        timer = Random.Range(3f, 5f);
    }

    private IEnumerator ConductSummonAttack()
    {
        timer = 500;

        if (isweapon)
        {
            anim.SetTrigger("PutWeapon");

            yield return new WaitForSeconds(2f);

            move.stop = true;
        }

        anim.SetBool("Move", false);
        anim.SetBool("Conduct", true);

        Instantiate(effects[17], weapon.transform.position, Quaternion.identity);

        for (int i = 0; i < Random.Range(5,10); i++)
        {
            int x = Random.Range(-18, 18);
            int y = Random.Range(-18, 18);

            GameObject _summon = Instantiate(summon[Random.Range(0, 2)], new Vector3(transform.position.x + x, 0, transform.position.z + y), Quaternion.identity);
            _summon.transform.forward = (player.transform.position - _summon.transform.position).normalized;
        }

        yield return new WaitForSeconds(3f);

        anim.SetBool("Conduct", false);

        yield return new WaitForSeconds(2f);

        if (!isweapon)
        {
            anim.SetTrigger("GetWeapon");

            yield return new WaitForSeconds(2f);
        }

        timer = Random.Range(3f, 5f);
    }

    public void GetWeapon()
    {
        weapon.SetActive(true);
        isweapon = true;
        Instantiate(effects[15], weapon.transform.position, Quaternion.identity);
    }

    public void PutWeapon()
    {
        Instantiate(effects[16], weapon.transform.position, Quaternion.identity);
        isweapon = false;
        weapon.SetActive(false);
    }

    public void NowMove()
    {
        move.stop = false;
    }

    public void NowStop()
    {
        move.stop = true;
    }
}
