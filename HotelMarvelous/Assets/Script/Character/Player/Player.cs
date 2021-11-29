using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerStatus stat = new PlayerStatus();

    private GameObject attackRange;
    private GameObject deadEft;
    private GameObject curItemSkill = null;

    private bool isInvincible = false;

    #region [Movement]

    public bool stopAllMove = false;
    private bool isattack = false;
    private bool isconvzone = false;
    private bool isDown = false;
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
        attackRange = transform.Find("AttackRange").gameObject;

        if(ScenesManager.Instance.CheckScene() != "Lobby")
        {
            deadEft = transform.Find("04_Eft_Player_Death").gameObject;
            deadEft.SetActive(false);
        }

        GetItemInfo();
    }

    void Update()
    {
        if(stopAllMove)
        {
            return;
        }

        DialogChecker();
        GetUp();

        if (isconv || ScenesManager.Instance.isOption || isDown)
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
        Debug.Log(other.gameObject.name);

        if (!stopAllMove)
        {
            if (isattack)
            {
                if (other.CompareTag("Monster"))
                {
                    float damage = DataManager.Instance.CalculateDamage();

                    other.GetComponent<Monster>().MonGetDamage(damage);
                }
                else if (other.CompareTag("Boss"))
                {
                    float damage = DataManager.Instance.CalculateDamage();

                    other.GetComponent<BossMonster>().MonGetDamage(damage);
                }
            }

            if (other.CompareTag("Dialoguezone"))
            {
                Interactionzone textinfo = other.GetComponent<Interactionzone>();

                if (textinfo.portalType == INTERACTION.NONE)
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
                RewardItem reward = other.transform.GetComponent<RewardItem>();

                byte id = (byte)reward.id;

                reward.ChangeItem((CONSUMITEM)stat.curItemIndex);
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
                }
                break;

            case DROPITEM.KEYS:
                if (stat.roomKeys < 100)
                {
                    stat.roomKeys++;
                }
                break;

            case DROPITEM.MASTERKEY:
                stat.roomKeys = 100;
                break;

            case DROPITEM.BEANS:
                if (stat.beans < 100)
                {
                    stat.beans++;
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
                break;
        }

        byte[] arr = new byte[] { 0, 1, 1, 1, 2, 2, 2, 3, 3, 3, 2, 2, 2 };

        GameObject[] effects = Resources.LoadAll<GameObject>("Effect/");

        GameObject effect = Instantiate(effects[arr[(byte)_consumitem.consumitem]], transform.position, Quaternion.identity);
        effect.transform.SetParent(transform);
        Destroy(effect, 2.5f);
        Destroy(other.gameObject);
    }

    IEnumerator SkillPerfume(int _curcount, int _max)
    {
        int count = _curcount;

        if(count < _max)
        {
            count++;

            GameObject[] Monsters = GameObject.FindGameObjectsWithTag("Monster");

            for(int i = 0; i < Monsters.Length; i++)
            {
                if(transform.Find("Perfume") == null)
                {
                    GameObject[] effects = Resources.LoadAll<GameObject>("Effect/");

                    GameObject eft = Instantiate(effects[12], Monsters[i].transform.position, Quaternion.identity);
                    eft.name = "Perfume";
                    eft.transform.SetParent(Monsters[i].transform);
                }

                Monsters[i].GetComponent<Monster>().MonGetDamage(Random.Range(4, 8));
            }

            yield return new WaitForSeconds(Random.Range(0.8f, 1f));

            StartCoroutine(SkillPerfume(count, _max));
        }
        else
        {
            yield return null;
        }
    }

    #endregion

    #region ----------------------------[Public]----------------------------

    public void ItemCounerAdd()
    {
        stat.curItemStack++;
    }
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

    public void SetDamage(float _damage)
    {
        if (isInvincible || isattack || stopAllMove)
        {
            return;
        }

        StatUI.profiletimer = 0;

        float truedamage = _damage * (1 + stat.defense / 100);

        stat.hp -= truedamage;
        isInvincible = true;

        if(stat.hp <= 0)
        {
            if(stat.extraLife > 0)
            {
                anim.SetTrigger("Down");
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
        else
        {
            if (truedamage >= 10)
            {
                anim.SetTrigger("Down");
                isDown = true;
            }
            else
            {
                anim.SetTrigger("Hit");
            }
        }
    }

    IEnumerator RebirthProcess()
    {
        isInvincible = true;
        isDown = true;

        yield return new WaitForSeconds(4.0f);

        isInvincible = false;
        isDown = false;
    }

    IEnumerator DeathProcess()
    {
        stopAllMove = true;
        anim.SetTrigger("Dead");

        deadEft.SetActive(true);

        yield return new WaitForSeconds(3.15f);

        deadEft.SetActive(false);

        yield return new WaitForSeconds(0.85f);

        ScenesManager.Instance.MoveToScene(INTERACTION.LOBBY);
    }

    public PlayerStatus GetStatus()
    {
        return stat;
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

    private void GetUp()
    {
        if(Input.anyKeyDown && isDown)
        {
            anim.SetTrigger("GetUp");
            isDown = false;
            stopAllMove = true;
        }
    }

    public void GetUpAlone()
    {
        isDown = false;
    }

    public void InvincibleOff()
    {
        isDown = false;
        isInvincible = false;
        stopAllMove = false;
    }

    #endregion

    #region ----------------------------[PlayerControl]----------------------------

    private void GetMovementInput()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.Mouse0) && !stopAllMove)
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
        if (Input.GetKeyDown(KeyCode.Mouse0) && !stopAllMove)
        {
            anim.SetTrigger("Attack");
        }
    }

    private void GetSkillInput()
    {
        if (Input.GetKey(ScenesManager.Instance.optionInfo.run) && ScenesManager.Instance.CheckScene() != "Lobby" && stat.hp > 10)
        {
            if (stat.stamina > 0)
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
            if(stat.hp > 10)
            {
                stat.speed = 1;
            }
            else
            {
                stat.speed = 0.5f;
            }
            

            if (stat.stamina <= 20)
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
            GameObject[] effects = Resources.LoadAll<GameObject>("Effect/");
            stat.curItemStack = 0;

            switch (stat.curItemIndex)
            {
                case 1:
                    anim.SetTrigger("Fire");

                    GameObject areaskill = Instantiate(curItemSkill, new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z), Quaternion.identity);
                    areaskill.transform.SetParent(transform);
                    WideAreaSkill wide = areaskill.GetComponent<WideAreaSkill>();
                    wide.SetSkillPreset("Monster", 20, 3f);

                    stopAllMove = true;
                    Invoke("InvincibleOff", 2.1f);
                    break;
                case 2:
                    GameObject butterfly = Instantiate(effects[10], transform.position, Quaternion.identity);
                    butterfly.transform.SetParent(transform);

                    if(stat.hp + 3 <= stat.maxHp)
                    {
                        stat.hp += 3;
                    }
                    else
                    {
                        stat.hp = stat.maxHp;
                    }

                    if(stat.mentality + 2 <= stat.maxMentality)
                    {
                        stat.mentality += 2;
                    }
                    else
                    {
                        stat.mentality = stat.maxMentality;
                    }
                    break;
                case 3:
                    GameObject perfume = Instantiate(effects[11], transform.position, Quaternion.identity);
                    perfume.transform.SetParent(transform);

                    StartCoroutine(SkillPerfume(0, 5));
                    break;
                case 4:
                    GameObject magicwallet = Instantiate(effects[12], transform.position, Quaternion.identity);
                    magicwallet.transform.SetParent(transform);

                    for (int i = 0; i < Random.Range(1,6); i++)
                    {
                        int x = Random.Range(-3, 4);
                        int z = Random.Range(-3, 4);

                        GameObject obj = ResourceManager.Instance.GetDropItem(DISPOITEM.COIN);
                        GameObject coin = Instantiate(obj, new Vector3(transform.position.x + x, 2, transform.position.z + z), Quaternion.identity);
                        ConsumItem conitem = coin.AddComponent<ConsumItem>();
                        conitem.consumitem = DROPITEM.COIN;
                    }
                    break;
                case 5:
                    GameObject raindow = Instantiate(effects[13], transform.position, Quaternion.identity);
                    raindow.transform.SetParent(transform);

                    if (stat.hp + 10 <= stat.maxHp)
                    {
                        stat.hp += 10;
                    }
                    else
                    {
                        stat.hp = stat.maxHp;
                    }

                    if (stat.mentality + 5 <= stat.maxMentality)
                    {
                        stat.mentality += 5;
                    }
                    else
                    {
                        stat.mentality = stat.maxMentality;
                    }
                    break;
                case 6:
                    anim.SetTrigger("Lightning");

                    GameObject lightning = Instantiate(effects[14], new Vector3(transform.position.x, 5, transform.position.z), transform.rotation);
                    lightning.transform.SetParent(transform);

                    GameObject[] Monsters = GameObject.FindGameObjectsWithTag("Monster");

                    for (int i = 0; i < Monsters.Length; i++)
                    {
                        Monsters[i].GetComponent<Monster>().MonGetDamage(5);
                    }
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
        transform.position += transform.forward * velocity * Time.deltaTime * stat.speed;

        if (ScenesManager.Instance.CheckScene() != "Lobby")
        {
            anim.SetFloat("Speed", stat.speed);
        }
    }

    #endregion

    #region ----------------------------[Trigger]----------------------------

    public IEnumerator MoveInIntro(float _timer, Vector3 _dir)
    {
        float timer = 0;

        anim = transform.GetComponent<Animator>();

        stopAllMove = true;

        while (timer <= _timer)
        {
            timer += Time.deltaTime;
            anim.SetBool("Move", true);
            if(ScenesManager.Instance.CheckScene() != "Lobby")
            anim.SetFloat("Speed", 1f);
            transform.position += _dir * Time.deltaTime * 3;

            yield return null;
        }

        anim.SetBool("Move", false);
        stopAllMove = false;
        yield return null;
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
