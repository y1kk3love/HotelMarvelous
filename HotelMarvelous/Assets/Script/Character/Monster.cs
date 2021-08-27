using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    public GameObject rangeskill;

    #region [DropItem]

    private GameObject obCoin;
    private GameObject obKey;
    private GameObject obMasterKey;
    private GameObject obBean;
    private GameObject obHealHp;
    private GameObject obHealMental;
    private GameObject obHealAll;

    #endregion

    private GameObject player;

    private NavMeshAgent navi;

    private Vector3 targetpos;

    public int monsterid;

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
            case 0:
                touchdamage = 5;
                monsterhp = 25;
                monspeed = 0.9f;
                mentaldamage = 0.5f;
                raylength = 5;
                break;
            case 1:
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
            //SpawnItem();
            Destroy(gameObject);
        }
    }

    private void SpawnItem()
    {
        //소모성 재화
        if(Random.Range(0, 100) < 40)                                                                   //40%
        {
            int _stateper = Random.Range(0, 100);

            if (_stateper < 50)                                         //50% 전체금화
            {
                int _coinper = Random.Range(0, 100);

                if(_coinper < 89)           //89% 금화 1개
                {
                    GameObject _coin = Instantiate(obCoin, new Vector3(transform.position.x, 0.25f, transform.position.z), Quaternion.identity);
                    ConsumItem conitem = _coin.AddComponent<ConsumItem>();
                    conitem.consumitem = DROPITEM.COIN;
                }
                else if (_coinper < 96)     //7% 금화 2개
                {
                    for(int i = 0; i < 2; i++)
                    {
                        GameObject _coin = Instantiate(obCoin, new Vector3(transform.position.x, 0.25f, transform.position.z), Quaternion.identity);
                        ConsumItem conitem = _coin.AddComponent<ConsumItem>();
                        conitem.consumitem = DROPITEM.COIN;
                    }
                }
                else                        //4% 금화 5개
                {
                    for (int i = 0; i < 5; i++)
                    {
                        GameObject _coin = Instantiate(obCoin, new Vector3(transform.position.x, 0.25f, transform.position.z), Quaternion.identity);
                        ConsumItem conitem = _coin.AddComponent<ConsumItem>();
                        conitem.consumitem = DROPITEM.COIN;
                    }
                }
            }
            else if (_stateper < 80)                                    //30% 전체열쇠
            {
                int _keyper = Random.Range(0, 100);

                if(_keyper < 99)            //99% 열쇠 1개
                {
                    GameObject _key = Instantiate(obKey, new Vector3(transform.position.x, 0.25f, transform.position.z), Quaternion.identity);
                    ConsumItem conitem = _key.AddComponent<ConsumItem>();
                    conitem.consumitem = DROPITEM.KEYS;
                }
                else                        //1% 마스터키
                {
                    GameObject _masterkey = Instantiate(obMasterKey, new Vector3(transform.position.x, 0.25f, transform.position.z), Quaternion.identity);
                    ConsumItem conitem = _masterkey.AddComponent<ConsumItem>();
                    conitem.consumitem = DROPITEM.MASTERKEY;
                }
            }
            else                                                        //20% 전체원두
            {
                int _beanper = Random.Range(0, 100);

                if (_beanper < 89)          //89% 원두 1개
                {
                    GameObject _bean = Instantiate(obBean, new Vector3(transform.position.x, 0.25f, transform.position.z), Quaternion.identity);
                    ConsumItem conitem = _bean.AddComponent<ConsumItem>();
                    conitem.consumitem = DROPITEM.BEANS;
                }
                else if(_beanper < 96)      //7% 원두 2개
                {
                    for (int i = 0; i < 2; i++)
                    {
                        GameObject _bean = Instantiate(obBean, new Vector3(transform.position.x, 0.25f, transform.position.z), Quaternion.identity);
                        ConsumItem conitem = _bean.AddComponent<ConsumItem>();
                        conitem.consumitem = DROPITEM.BEANS;
                    }
                }
                else                        //4% 원두 5개
                {
                    for (int i = 0; i < 5; i++)
                    {
                        GameObject _bean = Instantiate(obBean, new Vector3(transform.position.x, 0.25f, transform.position.z), Quaternion.identity);
                        ConsumItem conitem = _bean.AddComponent<ConsumItem>();
                        conitem.consumitem = DROPITEM.BEANS;
                    }
                }
            }
        }

        //몬스터 드랍 회복 아이템
        if(Random.Range(0,100) < 10)                                                                    //10%
        {
            int totalhealper = Random.Range(0, 100);

            if(totalhealper < 45)                                       //45% 전체체력회복
            {
                int hphealper = Random.Range(0, 100);

                if(hphealper < 70)          //70% +3
                {
                    GameObject _hp = Instantiate(obHealHp, new Vector3(transform.position.x, 0.25f, transform.position.z), Quaternion.identity);
                    ConsumItem conitem = _hp.AddComponent<ConsumItem>();
                    conitem.consumitem = DROPITEM.HPS;
                }
                else if(hphealper < 95)     //25% +5
                {
                    GameObject _hp = Instantiate(obHealHp, new Vector3(transform.position.x, 0.25f, transform.position.z), Quaternion.identity);
                    ConsumItem conitem = _hp.AddComponent<ConsumItem>();
                    conitem.consumitem = DROPITEM.HPM;
                }
                else                        //5% +10
                {
                    GameObject _hp = Instantiate(obHealHp, new Vector3(transform.position.x, 0.25f, transform.position.z), Quaternion.identity);
                    ConsumItem conitem = _hp.AddComponent<ConsumItem>();
                    conitem.consumitem = DROPITEM.HPL;
                }
            }
            else if(totalhealper < 90)                                  //45% 전체정신력회복
            {
                int mentalper = Random.Range(0, 100);

                if(mentalper < 70)          //70% +0.5
                {
                    GameObject _mental = Instantiate(obHealMental, new Vector3(transform.position.x, 0.25f, transform.position.z), Quaternion.identity);
                    ConsumItem conitem = _mental.AddComponent<ConsumItem>();
                    conitem.consumitem = DROPITEM.MENTALS;
                }
                else if(mentalper < 95)     //25% +1.0
                {
                    GameObject _mental = Instantiate(obHealMental, new Vector3(transform.position.x, 0.25f, transform.position.z), Quaternion.identity);
                    ConsumItem conitem = _mental.AddComponent<ConsumItem>();
                    conitem.consumitem = DROPITEM.MENTALM;
                }
                else                        //5% +5.0
                {
                    GameObject _mental = Instantiate(obHealMental, new Vector3(transform.position.x, 0.25f, transform.position.z), Quaternion.identity);
                    ConsumItem conitem = _mental.AddComponent<ConsumItem>();
                    conitem.consumitem = DROPITEM.MENTALL;
                }
            }
            else                                                        //10% 전체체력,정신력회복
            {
                int totalper = Random.Range(0, 100);

                if(totalper < 80)           //80% +3 && +1.0
                {
                    GameObject _total = Instantiate(obHealAll, new Vector3(transform.position.x, 0.25f, transform.position.z), Quaternion.identity);
                    ConsumItem conitem = _total.AddComponent<ConsumItem>();
                    conitem.consumitem = DROPITEM.TOTALHEALS;

                }
                else if(totalper < 96)      //16% +5 && 3.0
                {
                    GameObject _total = Instantiate(obHealAll, new Vector3(transform.position.x, 0.25f, transform.position.z), Quaternion.identity);
                    ConsumItem conitem = _total.AddComponent<ConsumItem>();
                    conitem.consumitem = DROPITEM.TOTALHEALM;
                }
                else                        //4% +20 & +10.0
                {
                    GameObject _total = Instantiate(obHealAll, new Vector3(transform.position.x, 0.25f, transform.position.z), Quaternion.identity);
                    ConsumItem conitem = _total.AddComponent<ConsumItem>();
                    conitem.consumitem = DROPITEM.TOTALHEALL;
                }
            }
        }
    }

    private void MonsterMove()
    {
        timer -= Time.deltaTime;

        switch (monsterid)
        {
            case 0:
                if (timer < 0.0f)
                {
                    MoveTargetPlayer();

                    timer = Random.Range(0.3f, 1);      //초기화할때 랜덤으로
                }
                break;
            case 1:
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
                navi.SetDestination(Hit.transform.position);
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
