using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerStatus stat = new PlayerStatus();

    public GameObject attackRange;
    private GameObject curItemSkill = null;

    private bool isInvincible = false;

    #region [Movement]

    private bool isattack = false;
    private bool isconvzone = false;
    public bool isconv = false;

    private byte curattack = 0;

    private Animator anim;

    private float velocity = 5f;
    private float turnspeed = 10f;

    private Vector2 input;
    private float angle;

    private Quaternion targetrotation;

    #endregion

    void Start()
    {
        stat = DataManager.Instance.GetPlayerStatus();
        anim = transform.GetComponent<Animator>();

        GetItemInfo();
    }

    void Update()
    {
        DialogChecker();

        if (isconv || ScenesManager.Instance.isOption)
        {
            anim.SetBool("Move", false);
            return;
        }

        AttackInput();

        if (isattack)
        {
            anim.SetBool("Move", false);
            return;
        }

        GetMovementInput();
        GetSkillInput();

        if (Mathf.Abs(input.x) < 1 && Mathf.Abs(input.y) < 1)
        {
            anim.SetBool("Move", false);
            return;
        }

        CalculateDirection();
        Rotate();
        Move();
    }

    #region ----------------------------[Trigger]----------------------------

    void OnTriggerEnter(Collider other)
    {
        if (isattack)
        {
            if (other.CompareTag("Monster") || other.CompareTag("Boss"))
            {
                float damage = DataManager.Instance.CalculateDamage();

                other.GetComponent<Monster>().MonGetDamage(damage);
            }
        }

        if (other.CompareTag("Dialoguezone"))
        {
            Interactionzone textinfo = other.GetComponent<Interactionzone>();

            if(textinfo.portalType == INTERACTION.NONE)
            {
                int point = (int)textinfo.dialogPoint;
                int index = textinfo.dialogIndex;

                ScenesManager.Instance.SetDialogPointInfo(point, index);

                isconvzone = true;

                ScenesManager.Instance.DialogEnter(point);

                Debug.Log("대화 장소에 입장");
            }
        }

        if (other.CompareTag("RewardItem"))
        {
            byte id = (byte)other.transform.GetComponent<RewardItem>().id;

            other.transform.GetComponent<RewardItem>().id = (CONSUMITEM)stat.curItemIndex;
            stat.curItemIndex = id;

            GetItemInfo();
        }

        if (other.CompareTag("ConsumItem"))
        {
            if (!isattack)
            {
                CheckDropItem(other);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Dialoguezone"))
        {
            ScenesManager.Instance.SetDialogPointInfo(-1, -1);
            isconvzone = false;
            ScenesManager.Instance.EntranceDecision(false);

            Debug.Log("대화 장소에 퇴장");
        }
    }

    #endregion

    #region ----------------------------[Private]----------------------------

    private void GetItemInfo()
    {
        ItemResource item = ResourceManager.Instance.GetItemResource(stat.curItemIndex);

        stat.curItemMax = item.max;
        curItemSkill = item.skillprefab;
    }

    private void CheckDropItem(Collider other)
    {
        ConsumItem _consumitem = other.GetComponent<ConsumItem>();

        switch (_consumitem.consumitem)
        {
            case DROPITEM.COIN:
                if (stat.coin < 100)
                {
                    stat.coin++;
                    Destroy(other.gameObject);
                }
                break;
            case DROPITEM.KEYS:
                if (stat.roomKeys < 100)
                {
                    stat.roomKeys++;
                    Destroy(other.gameObject);
                }
                break;
            case DROPITEM.MASTERKEY:
                stat.roomKeys = 100;
                Destroy(other.gameObject);
                break;
            case DROPITEM.BEANS:
                if (stat.beans < 100)
                {
                    stat.beans++;
                    Destroy(other.gameObject);
                }
                break;
            case DROPITEM.HPS:
                if (stat.hp + 3 <= stat.maxHp)
                {
                    stat.hp += 3;
                }
                else
                {
                    stat.hp = stat.maxHp;
                }

                Destroy(other.gameObject);
                break;
            case DROPITEM.HPM:
                if (stat.hp + 5 <= stat.maxHp)
                {
                    stat.hp += 5;
                }
                else
                {
                    stat.hp = stat.maxHp;
                }

                Destroy(other.gameObject);
                break;
            case DROPITEM.HPL:
                if (stat.hp + 10 <= stat.maxHp)
                {
                    stat.hp += 10;
                }
                else
                {
                    stat.hp = stat.maxHp;
                }

                Destroy(other.gameObject);
                break;
            case DROPITEM.MENTALS:
                if (stat.mentality + 0.5f <= stat.maxMentality)
                {
                    stat.mentality += 0.5f;
                }
                else
                {
                    stat.mentality = stat.maxMentality;
                }

                Destroy(other.gameObject);
                break;
            case DROPITEM.MENTALM:
                if (stat.mentality + 1.0f <= stat.maxMentality)
                {
                    stat.mentality += 1.0f;
                }
                else
                {
                    stat.mentality = stat.maxMentality;
                }

                Destroy(other.gameObject);
                break;
            case DROPITEM.MENTALL:
                if (stat.mentality + 5.0f <= stat.maxMentality)
                {
                    stat.mentality += 5.0f;
                }
                else
                {
                    stat.mentality = stat.maxMentality;
                }

                Destroy(other.gameObject);
                break;
            case DROPITEM.TOTALHEALS:
                if (stat.hp + 3 <= stat.maxHp)
                {
                    stat.hp += 3;
                }
                else
                {
                    stat.hp = stat.maxHp;
                }

                if (stat.mentality + 1.0f <= stat.maxMentality)
                {
                    stat.mentality += 1.0f;
                }
                else
                {
                    stat.mentality = stat.maxMentality;
                }

                Destroy(other.gameObject);
                break;
            case DROPITEM.TOTALHEALM:
                if (stat.hp + 5 <= stat.maxHp)
                {
                    stat.hp += 5;
                }
                else
                {
                    stat.hp = stat.maxHp;
                }

                if (stat.mentality + 3.0f <= stat.maxMentality)
                {
                    stat.mentality += 3.0f;
                }
                else
                {
                    stat.mentality = stat.maxMentality;
                }

                Destroy(other.gameObject);
                break;
            case DROPITEM.TOTALHEALL:
                if (stat.hp + 20 <= stat.maxHp)
                {
                    stat.hp += 20;
                }
                else
                {
                    stat.hp = stat.maxHp;
                }

                if (stat.mentality + 10.0f <= stat.maxMentality)
                {
                    stat.mentality += 10.0f;
                }
                else
                {
                    stat.mentality = stat.maxMentality;
                }

                Destroy(other.gameObject);
                break;
        }
    }

    #endregion

    #region ----------------------------[Public]----------------------------

    public void SetKey(bool _add)
    {
        if (_add)
        {
            stat.roomKeys++;
        }
        else
        {
            stat.roomKeys--;
        }
    }

    public void SetCoin(byte _coin, bool _add)
    {
        if (_add)
        {
            stat.coin += _coin;
        }
        else
        {
            stat.coin -= _coin;
        }
    }

    public void SetMentality(float _damage)
    {
        if (isInvincible)
        {
            return;
        }

        stat.mentality -= _damage;
    }

    public void SetDamage(int _damage)
    {
        if (isInvincible || isattack)
        {
            return;
        }

        stat.hp -= _damage * (1 + stat.defense / 100);

        if(stat.hp <= 0)
        {
            if(stat.extraLife > 0)
            {
                stat.extraLife--;
                stat.hp = stat.maxHp / 2;
                stat.maxHp = stat.maxHp / 2;

                StartCoroutine(RebirthProcess());
            }
            else
            {
                stat.hp = 0;

                StartCoroutine(DeathProcess());
            }
        }
    }

    IEnumerator RebirthProcess()
    {
        isInvincible = true;

        //무적 모션 시작자리

        yield return new WaitForSeconds(1.0f);

        isInvincible = false;
    }

    IEnumerator DeathProcess()
    {
        //사망모션 넣을 자리

        yield return new WaitForSeconds(0.1f);

        ScenesManager.Instance.MoveToScene(INTERACTION.LOBBY);
    }

    public PlayerStatus GetStatus()
    {
        return stat;
    }

    public void Test_ItemCounerAdd()
    {
        stat.curItemStack++;
    }

    public void Test_ChangeDispoItem(byte _index)
    {
        stat.curDispoItemIndex = _index;
    }

    #endregion

    #region ----------------------------[Animation]----------------------------

    private void Attack_01_Enter()
    {
        curattack = 0;
        isattack = true;
        AttackRange(0.3f);
    }

    private void Attack_02_Enter()
    {
        curattack = 1;
        isattack = true;
        AttackRange(0.3f);
    }

    private void Attack_03_Enter()
    {
        curattack = 2;
        isattack = true;
        AttackRange(0.4f);
    }

    private void AttackRange(float time)
    {
        attackRange.transform.GetChild(curattack).gameObject.SetActive(true);

        Invoke("AttackFinish", time);
    }

    private void AttackFinish()
    {
        attackRange.transform.GetChild(curattack).gameObject.SetActive(false);

        isattack = false;

        if (curattack <= 2)
        {
            curattack = 0;
        }
    }

    #endregion

    #region ----------------------------[PlayerControl]----------------------------

    private void GetMovementInput()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Vector3 targetPos = hit.point;
                Vector3 dir = targetPos - transform.position;
                transform.forward = new Vector3(dir.x, 0, dir.z).normalized;
            }
        }
    }

    private void DialogChecker()
    {
        if (Input.GetKeyDown(KeyCode.E) && isconvzone)
        {
            isconv = true;

            if (!ScenesManager.Instance.isOnChoice && !ScenesManager.Instance.isimtalking)
            {
                ScenesManager.Instance.DialogProcess();
            }
            else if(ScenesManager.Instance.isimtalking)
            {
                ScenesManager.Instance.MonologueProcess();
            }
        }
    }

    private void AttackInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            anim.SetTrigger("Attack");
        }
    }

    private void GetSkillInput()
    {
        if (Input.GetKey(ScenesManager.Instance.optionInfo.run))
        {
            if(stat.stamina > 0)
            {
                stat.stamina -= Time.deltaTime;
                stat.speed = stat.runspeed;
            }
            else
            {
                stat.stamina = 0;
                stat.speed = 1;
            }
        }
        else
        {
            stat.speed = 1;

            if(stat.stamina <= 20)
            {
                stat.stamina += Time.deltaTime / 3;
            }
            else
            {
                stat.stamina = 20;
            }
        }

        float _itemrecharge = (float)stat.curItemStack / (float)stat.curItemMax;

        if (Input.GetKeyDown(ScenesManager.Instance.optionInfo.recharge) && _itemrecharge >= 1)
        {
            Debug.Log("재사용 아이템 사용!");

            stat.curItemStack = 0;

            switch (stat.curItemIndex)
            {
                case 1:
                    GameObject areaskill = Instantiate(curItemSkill, new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z), Quaternion.identity);
                    WideAreaSkill wide = areaskill.GetComponent<WideAreaSkill>();
                    wide.SetSkillPreset("Monster", 3f);
                    break;
                case 2:
                    byte[] consumItemperArr = new byte[] { 40, 40, 20 };

                    int random = Random.Range(0, 100);
                    int curpercent = 0;

                    for (int i = 0; i < 3; i++)
                    {
                        curpercent += consumItemperArr[i];

                        if (random < curpercent)
                        {
                            switch (i)
                            {
                                case 0:
                                    consumItemperArr = new byte[] { 59, 30, 7, 3, 1 };

                                    random = Random.Range(0, 100);
                                    curpercent = 0;
                                    for (int x = 0; x < 5; x++)
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
                                                case 3:
                                                    _coin = 10;
                                                    break;
                                                case 4:
                                                    _coin = 100;
                                                    break;
                                            }

                                            for (int _x = 0; _x < _coin; _x++)
                                            {
                                                GameObject obj = ResourceManager.Instance.GetDropItem(DISPOITEM.COIN);
                                                GameObject coin = Instantiate(obj, new Vector3(transform.position.x, 0.25f, transform.position.z), Quaternion.identity);
                                                ConsumItem conitem = coin.AddComponent<ConsumItem>();
                                                conitem.consumitem = DROPITEM.COIN;
                                            }
                                        }
                                    }
                                    break;
                                case 1:
                                    random = Random.Range(0, 100);

                                    if (random < 95)
                                    {
                                        GameObject obj = ResourceManager.Instance.GetDropItem(DISPOITEM.KEY);
                                        GameObject _key = Instantiate(obj, new Vector3(transform.position.x, 0.25f, transform.position.z), Quaternion.identity);
                                        ConsumItem conitem = _key.AddComponent<ConsumItem>();
                                        conitem.consumitem = DROPITEM.KEYS;
                                    }
                                    else
                                    {
                                        GameObject obj = ResourceManager.Instance.GetDropItem(DISPOITEM.MASTERKEY);
                                        GameObject _masterkey = Instantiate(obj, new Vector3(transform.position.x, 0.25f, transform.position.z), Quaternion.identity);
                                        ConsumItem conitem = _masterkey.AddComponent<ConsumItem>();
                                        conitem.consumitem = DROPITEM.MASTERKEY;
                                    }
                                    break;
                                case 2:
                                    //타로 카드 드롭
                                    break;
                            }
                        }
                    }
                    Debug.Log("슬롯머신 발동!");
                    break;
            }
        }

        if (Input.GetKeyDown(ScenesManager.Instance.optionInfo.disposable) && stat.curDispoItemIndex != 255)
        {
            Debug.Log("일회용 아이템 사용!");

            if (_itemrecharge != 1)
            {
                switch (stat.curDispoItemIndex)
                {
                    case 1:
                        stat.curItemStack = stat.curItemMax;
                        stat.curDispoItemIndex = 255;
                        break;
                }
            }
        }
    }

    private void Rotate()
    {
        targetrotation = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetrotation, turnspeed * Time.deltaTime);
    }

    private void Move()
    {
        anim.SetBool("Move", true);
        anim.SetFloat("Speed", stat.speed);
        transform.position += transform.forward * velocity * Time.deltaTime * stat.speed;
    }

    #endregion

    #region ----------------------------[Calculate]----------------------------

    private void CalculateDirection()
    {
        angle = Mathf.Atan2(input.x, input.y);
        angle = Mathf.Rad2Deg * angle;
        angle += Camera.main.transform.eulerAngles.y;
    }

    #endregion
}
