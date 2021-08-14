using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerStatus stat = new PlayerStatus();

    private bool isInvincible = false;

    #region [Item]

    private GameObject curItemSkill = null;


    private float itemMagnification = 6;
    private float itemcount = 1;
    private bool isdispoitemon = false;
    private bool isMasterKey = false;
    private byte dispoitemcode = 0;
    private byte itemcode = 0;

    #endregion

    #region [Weapon]

    private List<GameObject> atkrangeList = new List<GameObject>();

    public WEAPONID WeaponID = WEAPONID.SWORD;

    #endregion

    #region [TextZone]

    private bool isconv = false;
    private bool ontextzone = false;
    private GameObject obzone;

    #endregion

    #region [Movement]

    private bool isattack = false;

    private Animator anim;

    private float velocity = 5f;
    private float turnspeed = 10f;

    private Vector2 input;
    private float angle;

    private Quaternion targetrotation;

    #endregion

    void Start()
    {
        anim = transform.GetComponent<Animator>();
    }

    void Update()
    {
        if (isconv || !isattack || !ScenesManager.Instance.onOption)
        {
            anim.SetBool("Run", false);
            return;
        }

        GetMovementInput();
        GetSkillInput();

        if (Mathf.Abs(input.x) < 1 && Mathf.Abs(input.y) < 1)
        {
            anim.SetBool("Run", false);
            return;
        }

        CalculateDirection();
        Rotate();
        Move();
    }

    #region ----------------------------[Trigger]----------------------------

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Dialoguezone>())
        {
            ontextzone = true;
            obzone = other.gameObject;
            Debug.Log("대화 장소에 입장");
        }

        if (other.GetComponent<RewardItem>())
        {
            ResourceManager.Instance.GetItemResource(stat.curItemIndex);
        }

        if (other.gameObject.tag == "ConsumItem")
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
                    isMasterKey = true;
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
                    if(stat.hp + 3 <= stat.maxHp)
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

        if(other.gameObject.name == "MoveLift")
        {
            //다음 층으로 이동
            Debug.Log("다음층으로 이동");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Dialoguezone>())
        {
            ontextzone = false;
            Debug.Log("나가요~");
        }
    }

    #endregion

    #region ----------------------------[Public]----------------------------

    public void Test_AddItemCount()
    {
        itemcount++;
    }

    public void Test_AddDisoItem()
    {
        isdispoitemon = true;
        dispoitemcode = 1;
    }

    public void Test_GamePause()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    public void SetKey(bool _add)
    {
        if (_add)
        {
            if (isMasterKey)
            {
                stat.roomKeys = 100;
            }
            else
            {
                stat.roomKeys++;
            }
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

    public void SetDamage(int _damage)
    {
        if (isInvincible)
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

        ScenesManager.Instance.MoveToScene("Lobby");
    }

    #endregion

    #region ----------------------------[Animation]----------------------------

    private void Attack_00_Enter()
    {
        atkrangeList[0].SetActive(true);
        atkrangeList[0].GetComponent<AttackTrigger>().SetDamage(CalculateDamagePercent());
    }

    private void Attack_00_Exit()
    {
        atkrangeList[0].SetActive(false);
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

    private void GetSkillInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartCoroutine(Attack_00());
        }

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

        if (Input.GetKeyDown(ScenesManager.Instance.optionInfo.recharge) && itemcount / itemMagnification >= 1)
        {
            itemcount = 0;

            switch (itemcode)
            {
                case 0:
                    GameObject areaskill = Instantiate(ResourceManager.Instance.GetSkillPrefeb((byte)ITEMCODE.CROWN), transform.position, Quaternion.identity);
                    WideAreaSkill wide = areaskill.GetComponent<WideAreaSkill>();
                    wide.SetSkillPreset("Monster", 3f);
                    break;
                case 1:
                    Debug.Log("슬롯머신 발동!");
                    break;
            }
        }

        if (Input.GetKeyDown(ScenesManager.Instance.optionInfo.disposable) && isdispoitemon)
        {
            if (itemcount / itemMagnification != 1)
            {
                switch (dispoitemcode)
                {
                    case 1:
                        itemcount = itemMagnification;
                        isdispoitemon = false;
                        dispoitemcode = 0;
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
        anim.SetBool("Run", true);
        transform.position += transform.forward * velocity * Time.deltaTime * stat.speed;
    }

    IEnumerator Attack_00()
    {
        anim.SetBool("Attack_00", true);
        isattack = true;

        yield return new WaitForSeconds(1.02f);

        anim.SetBool("Attack_00", false);
        isattack = false;
    }
    #endregion

    #region ----------------------------[Calculate]----------------------------

    private void CalculateDirection()
    {
        angle = Mathf.Atan2(input.x, input.y);
        angle = Mathf.Rad2Deg * angle;
        angle += Camera.main.transform.eulerAngles.y;
    }

    private float CalculateDamagePercent()
    {
        int _percent = Random.Range(0, 100);
        float _damage = stat.damage;

        if (_percent < stat.missdam)
        {
            _damage -= 2;
        }
        else if (_percent < stat.missdam + stat.lightdam)
        {
            _damage--;
        }
        else if (_percent < stat.missdam + stat.lightdam + stat.normaldam)
        {
            return _damage;
        }
        else if (_percent < stat.missdam + stat.lightdam + stat.normaldam + stat.harddam)
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

        if (curpercent < stat.criticalPercent * 10)
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
