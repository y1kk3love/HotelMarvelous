using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    #region [Status]

    private int damage = 30;
    private int damperm2, damperm1, dampern, damperp1;
    private float hp = 0;
    private float maxHp = 30;
    private float criticalPercent = 3.0f;
    private float stamina = 20.0f;
    private float runspeed = 1.5f;
    private float speed = 1f;
    private float mentality = 0.0f;
    private float maxMentality = 20.0f;
    private byte extraLife = 1;
    private int defense = 5;

    #endregion

    #region [Item]

    private byte coin = 0;
    private byte roomKeys = 0;
    private byte beans = 0;

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
        anim = gameObject.GetComponent<Animator>();

        damperm2 = 15;
        damperm1 = 15;
        dampern = 40;
        damperp1 = 15;

        hp = maxHp;
        mentality = maxMentality;
    }

    void Update()
    {
        if (isMasterKey)
        {
            roomKeys = 100;
        }

        if(hp <= 0) 
        {
            if(extraLife == 1)
            {
                //사망처리
            }
            else if(extraLife > 1)
            {
                extraLife--;

                if(maxHp/2 <= 5)
                {
                    maxHp = 5;
                }
                else
                {
                    maxHp /= 2;
                }
            }
        }

        if (isconv)
        {
            anim.SetBool("Run", false);
            return;
        }

        if (!isattack && !ScenesManager.Instance.onOption)
        {
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
        else
        {
            return;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Dialoguezone>())
        {
            ontextzone = true;
            obzone = other.gameObject;
            Debug.Log("들어왔어요~");
        }
        if (other.GetComponent<RewardItem>())
        {
            byte _itemcode;

            RewardItem rewarditem = other.GetComponent<RewardItem>();

            _itemcode = rewarditem.GetRewardItemcode();
            rewarditem.SetRewardItemcode(itemcode);
            itemcode = _itemcode;

            itemMagnification = ResourceManager.Instance.GetItemMagnification(itemcode);
        }
        if (other.gameObject.tag == "ConsumItem")
        {
            ConsumItem _consumitem = other.GetComponent<ConsumItem>();

            switch (_consumitem.consumitem)
            {
                case DROPITEM.COIN:
                    if (coin < 100)
                    {
                        coin++;
                        Destroy(other.gameObject);
                    }
                    break;
                case DROPITEM.KEYS:
                    if (roomKeys < 100)
                    {
                        roomKeys++;
                        Destroy(other.gameObject);
                    }
                    break;
                case DROPITEM.MASTERKEY:
                    isMasterKey = true;
                    Destroy(other.gameObject);
                    break;
                case DROPITEM.BEANS:
                    if (beans < 100)
                    {
                        beans++;
                        Destroy(other.gameObject);
                    }
                    break;
                case DROPITEM.HPS:
                    if(hp + 3 <= maxHp)
                    {
                        hp += 3;
                    }
                    else
                    {
                        hp = maxHp;
                    }

                    Destroy(other.gameObject);
                    break;
                case DROPITEM.HPM:
                    if (hp + 5 <= maxHp)
                    {
                        hp += 5;
                    }
                    else
                    {
                        hp = maxHp;
                    }

                    Destroy(other.gameObject);
                    break;
                case DROPITEM.HPL:
                    if (hp + 10 <= maxHp)
                    {
                        hp += 10;
                    }
                    else
                    {
                        hp = maxHp;
                    }

                    Destroy(other.gameObject);
                    break;
                case DROPITEM.MENTALS:
                    if (mentality + 0.5f <= maxMentality)
                    {
                        mentality += 0.5f;
                    }
                    else
                    {
                        mentality = maxMentality;
                    }

                    Destroy(other.gameObject);
                    break;
                case DROPITEM.MENTALM:
                    if (mentality + 1.0f <= maxMentality)
                    {
                        mentality += 1.0f;
                    }
                    else
                    {
                        mentality = maxMentality;
                    }

                    Destroy(other.gameObject);
                    break;
                case DROPITEM.MENTALL:
                    if (mentality + 5.0f <= maxMentality)
                    {
                        mentality += 5.0f;
                    }
                    else
                    {
                        mentality = maxMentality;
                    }

                    Destroy(other.gameObject);
                    break;
                case DROPITEM.TOTALHEALS:
                    if (hp + 3 <= maxHp)
                    {
                        hp += 3;
                    }
                    else
                    {
                        hp = maxHp;
                    }

                    if (mentality + 1.0f <= maxMentality)
                    {
                        mentality += 1.0f;
                    }
                    else
                    {
                        mentality = maxMentality;
                    }

                    Destroy(other.gameObject);
                    break;
                case DROPITEM.TOTALHEALM:
                    if (hp + 5 <= maxHp)
                    {
                        hp += 5;
                    }
                    else
                    {
                        hp = maxHp;
                    }

                    if (mentality + 3.0f <= maxMentality)
                    {
                        mentality += 3.0f;
                    }
                    else
                    {
                        mentality = maxMentality;
                    }

                    Destroy(other.gameObject);
                    break;
                case DROPITEM.TOTALHEALL:
                    if (hp + 20 <= maxHp)
                    {
                        hp += 20;
                    }
                    else
                    {
                        hp = maxHp;
                    }

                    if (mentality + 10.0f <= maxMentality)
                    {
                        mentality += 10.0f;
                    }
                    else
                    {
                        mentality = maxMentality;
                    }

                    Destroy(other.gameObject);
                    break;
            }
        }
        if(other.gameObject.name == "MoveLift")
        {
            DungeonUI du = GameObject.Find("UI").GetComponent<DungeonUI>();
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

    public void ExitTextZone()
    {
        ontextzone = false;
    }

    public void SetCoin(byte _coin, bool _add)
    {
        if (_add)
        {
            coin += _coin;
        }
        else
        {
            coin -= _coin;
        }
    }

    public float GetMaxMentality()
    {
        return maxMentality;
    }

    public float GetMaxHp()
    {
        return maxHp;
    }

    public byte GetBeans()
    {
        return beans;
    }

    public byte GetKeys()
    {
        return roomKeys;
    }

    public byte GetCoin()
    {
        return coin;
    }

    public float GetMentality()
    {
        return mentality;
    }

    public byte GetItemCode()
    {
        return itemcode;
    }   

    public float GetItemCount()
    {
        return itemcount / itemMagnification;
    }
    public float GetHp()
    {
        return hp;
    }

    public byte GetDispoItemCode()
    {
        if (isdispoitemon)
        {
            return dispoitemcode;
        }
        else
        {
            return 0;
        }
    }

    public float GetStamina()
    {
        return stamina;
    }

    public bool GetOnTextZone()
    {
        return ontextzone;
    }

    public GameObject GetObZone()
    {
        if(obzone != null)
        {
            return obzone;
        }
        else
        {
            return null;
        }
    }

    public void SetIsConversation(bool _bool)
    {
        isconv = _bool;
    }

    public void SetMentality(float _mentality)
    {
        mentality -= _mentality;
    }

    public void SetAtkRangList(GameObject _atkrang)
    {
        atkrangeList.Add(_atkrang);
    }

    public void SetItemMagnification(int _magnification)
    {
        itemMagnification = _magnification;
    }

    public void SetDamage(int _damage)
    {
        hp -= _damage * (1 + defense / 100);

        if(hp <= 0)
        {
            if(extraLife > 0)
            {
                extraLife--;
                hp = maxHp / 2;
                maxHp = maxHp / 2;
            }
            else
            {
                hp = 0;

                StartCoroutine(DeathProcess());
            }
        }
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
            if(stamina > 0)
            {
                stamina -= Time.deltaTime;
                speed = runspeed;
            }
            else
            {
                stamina = 0;
                speed = 1;
            }
        }
        else
        {
            speed = 1;

            if(stamina <= 20)
            {
                stamina += Time.deltaTime / 3;
            }
            else
            {
                stamina = 20;
            }
        }

        if (Input.GetKeyDown(ScenesManager.Instance.optionInfo.recharge) && itemcount / itemMagnification >= 1)
        {
            itemcount = 0;

            switch (itemcode)
            {
                case 0:
                    GameObject areaskill = Instantiate(ResourceManager.Instance.GetItemPrefeb((byte)ITEMCODE.CROWN), transform.position, Quaternion.identity);
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
        transform.position += transform.forward * velocity * Time.deltaTime * speed;
    }

    private void CalculateDirection()
    {
        angle = Mathf.Atan2(input.x, input.y);
        angle = Mathf.Rad2Deg * angle;
        angle += Camera.main.transform.eulerAngles.y;
    }

    private float CalculateDamagePercent()
    {
        int _percent = Random.Range(0, 100);
        float _damage = damage;

        if(_percent < damperm2)
        {
            _damage -= 2;
        }
        else if(_percent < damperm2 + damperm1)
        {
            _damage--;
        }
        else if(_percent < damperm2 + damperm1 + dampern)
        {
            return _damage;
        }
        else if(_percent < damperm2 + damperm1 + dampern + damperp1)
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

        if(curpercent < criticalPercent * 10)
        {
            return true;
        }
        else
        {
            return false;
        }
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
}
