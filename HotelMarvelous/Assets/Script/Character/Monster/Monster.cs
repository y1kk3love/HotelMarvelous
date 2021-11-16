using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    public GameObject ink;
    private GameObject player;
    private MonsterMove move;
    private Animator anim;

    public GameObject DeadEffect;
    public GameObject hitEffect;

    private bool isDeadOnce = false;
    private bool isdead = false;

    public int monsterid;

    private int touchdamage;
    private float mentaldamage; 
    private float monsterhp;
    private float timer = 0;
    private float attackrange = 5;
    
    void Start()
    {
        anim = transform.GetComponent<Animator>();
        move = transform.GetComponent<MonsterMove>();
        player = GameObject.FindGameObjectWithTag("Player");

        switch (monsterid)
        {
            case 0:
                touchdamage = 5;
                monsterhp = 25;
                move.speed = 5f;
                mentaldamage = 0.9f;
                attackrange = 2;
                break;
            case 1:
                touchdamage = 3;
                monsterhp = 22;
                move.speed = 0.7f;
                mentaldamage = 0f;
                attackrange = 8;
                break;
            case 2:
                touchdamage = 3;
                monsterhp = 22;
                move.speed = 0.7f;
                mentaldamage = 0f;
                attackrange = 8;
                break;
        }
    }

    void Update()
    {
        if(ScenesManager.Instance.CheckScene() == "MapTools")
        {
            return;
        }
        else
        {
            timer += Time.deltaTime;
        }

        MonsterAttack();
    }

    private void MonsterAttack()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        else
        {
            float dis = Vector3.Distance(player.transform.position, transform.position);

            if (!isdead && dis <= attackrange)
            {
                if (timer >= 2)
                {
                    timer = 0;

                    GameObject _effect = Instantiate(hitEffect, player.transform.position, Quaternion.identity);
                    Destroy(_effect, 1f);
                    anim.SetTrigger("Attack");
                    player.GetComponent<Player>().SetDamage(touchdamage);
                    player.GetComponent<Player>().SetMentality(mentaldamage);
                }
            }
        }
    }

    public void MonGetDamage(float _damage)
    {
        Debug.Log("Get " + _damage + " Damage");

        monsterhp -= _damage;

        if (monsterhp <= 0)
        {
            GameObject.Find("DungeonManager").GetComponent<DungeonMaker>().MonsterDead();
            SpawnConsumItem();
            SpawnHealItem();
            StartCoroutine(MonsterDead());
        }
    }

    IEnumerator MonsterDead()
    {
        isdead = true;       
        transform.GetComponent<MonsterMove>().isDead = true;

        if (isDeadOnce)
        { 
            anim.SetTrigger("Dead"); 
        }
        else
        {
            GameObject effect = Instantiate(DeadEffect, transform.position, Quaternion.Euler(-90, 0, 0));
            Destroy(effect, 1f);

            transform.Find("Body").gameObject.SetActive(false);

            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < 2; i++)
            {
                GameObject mon = Instantiate(ink, new Vector3(transform.position.x + i, 0.5f, transform.position.z), Quaternion.identity);
                GameObject.Find("DungeonManager").GetComponent<DungeonMaker>().MonsterAdd();
                mon.transform.GetComponent<Monster>().isDeadOnce = true;
                mon.transform.GetComponent<MonsterMove>().isDead = false;
            }

            Destroy(gameObject);
        }

         yield return new WaitForSeconds(1.1f);

        Destroy(gameObject);
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
}
