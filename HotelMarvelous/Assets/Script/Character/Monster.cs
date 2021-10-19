using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    public GameObject rangeskill;

    private GameObject player;

    private NavMeshAgent navi;

    private Vector3 targetpos;

    public MONSTERTYPE monsterid;

    private bool isdead = false;
    private bool isattacking = false;
    private bool isinstantiate = false;

    private int touchdamage;
    private float mentaldamage; 
    private float monsterhp;
    private float monspeed = 1;
    private float timer = 0.5f;
    private float raylength = 5;

    void Start()
    {
        navi = GetComponent<NavMeshAgent>();

        player = GameObject.FindGameObjectWithTag("Player");

        switch (monsterid)
        {
            case MONSTERTYPE.SUPERVIA:
                touchdamage = 5;
                monsterhp = 25;
                monspeed = 0.9f;
                mentaldamage = 0.5f;
                raylength = 5;
                break;
            case MONSTERTYPE.AVARITIA:
                touchdamage = 3;
                monsterhp = 22;
                monspeed = 0.7f;
                mentaldamage = 0f;
                raylength = 8;
                break;
        }

        if(navi != null)
        {
            navi.speed = monspeed * 5;
        }
    }

    void Update()
    {
        if(ScenesManager.Instance.CheckScene() == "MapTools")
        {
            return;
        }

        MonsterDeath();

        MonsterMove();

        DebugRay();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();

            player.SetDamage(touchdamage);
            player.SetMentality(mentaldamage);
        }
    }

    private void DebugRay()
    {
        Vector3 Debugdir = player.transform.position - transform.position;      //디버깅용
        Debugdir.Normalize();
        Debug.DrawRay(this.transform.position, Debugdir * raylength, Color.red);
    }

    public float GetBossHP()
    {
        return monsterhp;
    }

    public void MonGetDamage(float _damage)
    {
        Debug.Log("Get " + _damage + " Damage");

        monsterhp -= _damage;
    }

    private void MonsterDeath()
    {
        if (monsterhp <= 0 && !isdead)
        {
            isdead = true;
            GameObject.Find("DungeonMaker").GetComponent<DungeonMaker>().MonsterDead();
            navi = null;
            SpawnConsumItem();
            SpawnHealItem();
            Destroy(gameObject);
        }
    }

    #region [DropItem]

    private void SpawnHealItem()
    {
        byte[] consumItemperArr;

        int random = Random.Range(0, 100);
        int curpercent = 0;

        if (random < 10)
        {
            random = Random.Range(0, 100);

            for (int i = 0; i < 3; i++)
            {
                consumItemperArr = new byte[] { 45, 45, 10 };

                curpercent += consumItemperArr[i];

                if (random < curpercent)
                {
                    switch (i)
                    {
                        case 0:
                            random = Random.Range(0, 100);

                            for (int _x = 0; _x < 3; _x++)
                            {
                                consumItemperArr = new byte[] { 75, 25, 10 };

                                curpercent += consumItemperArr[i];

                                if (random < curpercent)
                                {
                                    GameObject obj = ResourceManager.Instance.GetDropItem(DISPOITEM.HP);

                                    switch (i)
                                    {
                                        case 0:
                                            GameObject s = Instantiate(obj, new Vector3(transform.position.x, 2, transform.position.z), Quaternion.identity);
                                            ConsumItem _s = s.AddComponent<ConsumItem>();
                                            _s.consumitem = DROPITEM.HPS;
                                            return;
                                        case 1:
                                            GameObject m = Instantiate(obj, new Vector3(transform.position.x, 2, transform.position.z), Quaternion.identity);
                                            ConsumItem _m = m.AddComponent<ConsumItem>();
                                            _m.consumitem = DROPITEM.HPM;
                                            return;
                                        case 2:
                                            GameObject l = Instantiate(obj, new Vector3(transform.position.x, 2, transform.position.z), Quaternion.identity);
                                            ConsumItem _l = l.AddComponent<ConsumItem>();
                                            _l.consumitem = DROPITEM.HPL;
                                            return;
                                    }
                                }
                            }
                            break;
                        case 1:
                            random = Random.Range(0, 100);

                            for (int _x = 0; _x < 3; _x++)
                            {
                                consumItemperArr = new byte[] { 75, 25, 10 };

                                curpercent += consumItemperArr[i];

                                if (random < curpercent)
                                {
                                    GameObject obj = ResourceManager.Instance.GetDropItem(DISPOITEM.MENTAL);

                                    switch (i)
                                    {
                                        case 0:
                                            GameObject s = Instantiate(obj, new Vector3(transform.position.x, 2, transform.position.z), Quaternion.identity);
                                            ConsumItem _s = s.AddComponent<ConsumItem>();
                                            _s.consumitem = DROPITEM.MENTALS;
                                            return;
                                        case 1:
                                            GameObject m = Instantiate(obj, new Vector3(transform.position.x, 2, transform.position.z), Quaternion.identity);
                                            ConsumItem _m = m.AddComponent<ConsumItem>();
                                            _m.consumitem = DROPITEM.MENTALM;
                                            return;
                                        case 2:
                                            GameObject l = Instantiate(obj, new Vector3(transform.position.x, 2, transform.position.z), Quaternion.identity);
                                            ConsumItem _l = l.AddComponent<ConsumItem>();
                                            _l.consumitem = DROPITEM.MENTALL;
                                            return;
                                    }
                                }
                            }
                            break;
                        case 2:
                            random = Random.Range(0, 100);

                            for (int _x = 0; _x < 3; _x++)
                            {
                                consumItemperArr = new byte[] { 80, 16, 4 };

                                curpercent += consumItemperArr[i];

                                if (random < curpercent)
                                {
                                    GameObject obj = ResourceManager.Instance.GetDropItem(DISPOITEM.HEALALL);

                                    switch (i)
                                    {
                                        case 0:
                                            GameObject s = Instantiate(obj, new Vector3(transform.position.x, 2, transform.position.z), Quaternion.identity);
                                            ConsumItem _s = s.AddComponent<ConsumItem>();
                                            _s.consumitem = DROPITEM.TOTALHEALS;
                                            return;
                                        case 1:
                                            GameObject m = Instantiate(obj, new Vector3(transform.position.x, 2, transform.position.z), Quaternion.identity);
                                            ConsumItem _m = m.AddComponent<ConsumItem>();
                                            _m.consumitem = DROPITEM.TOTALHEALM;
                                            return;
                                        case 2:
                                            GameObject l = Instantiate(obj, new Vector3(transform.position.x, 2, transform.position.z), Quaternion.identity);
                                            ConsumItem _l = l.AddComponent<ConsumItem>();
                                            _l.consumitem = DROPITEM.TOTALHEALL;
                                            return;
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }
    }

    private void SpawnConsumItem()
    {
        byte[] consumItemperArr;

        int random = Random.Range(0, 100);
        int curpercent = 0;

        if(random < 40)
        {
            random = Random.Range(0, 100);

            for (int i = 0; i < 3; i++)
            {
                consumItemperArr = new byte[] { 50, 30, 20 };

                curpercent += consumItemperArr[i];

                if (random < curpercent)
                {
                    switch (i)
                    {
                        case 0:
                            consumItemperArr = new byte[] { 89, 7, 4 };

                            random = Random.Range(0, 100);
                            curpercent = 0;

                            for (int x = 0; x < 3; x++)
                            {
                                curpercent += consumItemperArr[i];

                                if (random < curpercent)
                                {
                                    byte _coin = 0;

                                    switch (x)
                                    {
                                        case 0:
                                            _coin = 1;
                                            break;
                                        case 1:
                                            _coin = 2;
                                            break;
                                        case 2:
                                            _coin = 5;
                                            break;
                                    }

                                    for (int _x = 0; _x < _coin; _x++)
                                    {
                                        GameObject obj = ResourceManager.Instance.GetDropItem(DISPOITEM.COIN);
                                        GameObject coin = Instantiate(obj, new Vector3(transform.position.x, 2, transform.position.z), Quaternion.identity);
                                        ConsumItem conitem = coin.AddComponent<ConsumItem>();
                                        conitem.consumitem = DROPITEM.COIN;
                                    }

                                    return;
                                }
                            }
                            return;
                        case 1:
                            random = Random.Range(0, 100);

                            if (random < 99)
                            {
                                GameObject obj = ResourceManager.Instance.GetDropItem(DISPOITEM.KEY);
                                GameObject _key = Instantiate(obj, new Vector3(transform.position.x, 2, transform.position.z), Quaternion.identity);
                                ConsumItem conitem = _key.AddComponent<ConsumItem>();
                                conitem.consumitem = DROPITEM.KEYS;
                            }
                            else
                            {
                                GameObject obj = ResourceManager.Instance.GetDropItem(DISPOITEM.MASTERKEY);
                                GameObject _masterkey = Instantiate(obj, new Vector3(transform.position.x, 2, transform.position.z), Quaternion.identity);
                                ConsumItem conitem = _masterkey.AddComponent<ConsumItem>();
                                conitem.consumitem = DROPITEM.MASTERKEY;
                            }
                            return;
                        case 2:
                            consumItemperArr = new byte[] { 89, 7, 4 };

                            random = Random.Range(0, 100);
                            curpercent = 0;

                            for (int y = 0; y < 3; y++)
                            {
                                curpercent += consumItemperArr[i];

                                if (random < curpercent)
                                {
                                    byte _bean = 0;

                                    switch (y)
                                    {
                                        case 0:
                                            _bean = 1;
                                            break;
                                        case 1:
                                            _bean = 2;
                                            break;
                                        case 2:
                                            _bean = 5;
                                            break;
                                    }

                                    for (int _y = 0; _y < _bean; _y++)
                                    {
                                        GameObject obj = ResourceManager.Instance.GetDropItem(DISPOITEM.BEAN);
                                        GameObject coin = Instantiate(obj, new Vector3(transform.position.x, 2, transform.position.z), Quaternion.identity);
                                        ConsumItem conitem = coin.AddComponent<ConsumItem>();
                                        conitem.consumitem = DROPITEM.BEANS;
                                    }
                                }
                            }
                            return;
                    }
                }
            }
        }
    }

    #endregion

    private void MonsterMove()
    {
        timer -= Time.deltaTime;

        switch (monsterid)
        {
            case MONSTERTYPE.SUPERVIA:
                if (timer < 0.0f)
                {
                    MoveTargetPlayer();

                    timer = Random.Range(0.3f, 1);      //초기화할때 랜덤으로
                }
                break;
            case MONSTERTYPE.AVARITIA:
                if(navi != null)
                {
                    if (!isattacking)
                    {
                        navi.SetDestination(player.transform.position);
                        TargetFoward();
                    }
                    else
                    {
                        navi.SetDestination(transform.position);

                        if (!isinstantiate)
                        {
                            StartCoroutine(MonsterBeamSkill());
                        }

                        if (timer <= 0)
                        {
                            isattacking = false;
                        }
                    }
                }
                break;
        }
        
    }

    private void TargetFoward()
    {
        Vector3 Monpos = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);

        RaycastHit Hit;
        Physics.Raycast(Monpos, transform.forward, out Hit, raylength);

        if (Physics.Raycast(Monpos, transform.forward, out Hit, raylength))
        {
            if (Hit.transform == player.transform)
            {
                isattacking = true;
                timer = 6f;
                targetpos = Hit.transform.position;
            }
        }
    }

    private void MoveTargetPlayer()
    {
        Vector3 Monpos = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);

        RaycastHit Hit;
        Vector3 dir = player.transform.position - Monpos;           //방향값은 목표vector좌표에서 내 vector좌표를 뺀값
        Physics.Raycast(Monpos, dir, out Hit, raylength);

        if (Physics.Raycast(Monpos, dir, out Hit, raylength))
        {
            if (Hit.transform == player.transform)
            {
                //navi.SetDestination(Hit.transform.position);
            }
        }
    }

    IEnumerator MonsterBeamSkill()
    {
        isinstantiate = true;

        yield return new WaitForSeconds(1f);

        GameObject beam = Instantiate(rangeskill, targetpos, transform.rotation);
        WideAreaSkill wide = beam.GetComponent<WideAreaSkill>();
        wide.SetSkillPreset("Player", 5f);
        float range = Vector3.Distance(player.transform.position, transform.position);
        wide.SetSkillRange(range/5);

        yield return new WaitForSeconds(5f);

        isinstantiate = false;
    }
}
