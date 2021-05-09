using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WEAPONID
{
    SWORD,
    SPEAR
}

public enum ITEMCODE : byte
{
    CROWN,
    SLOTMACHINE
}

//[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    private ResourceManager resource;

    #region [Status]

    private int damage = 3;
    private int hp = 200;
    private float stamina = 20;
    private float runspeed = 1.5f;
    private float speed = 1f;
    private float mentality = 50.0f;

    #endregion

    #region [Item]
    private byte coin = 0;

    private float itemMagnification = 6;
    private float itemcount = 1;
    private bool isdispoitemon = false;
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

        if (GameObject.Find("ResourceManager") != null)
        {
            resource = GameObject.Find("ResourceManager").GetComponent<ResourceManager>();
        }
    }

    void Update()
    {
        if (isconv)
        {
            anim.SetBool("Run", false);
            return;
        }

        if (!isattack)
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

            itemMagnification = resource.GetItemMagnification(itemcode);
        }
        if (other.gameObject.name == "Coin")
        {
            if(coin < 100)
            {
                coin++;
                Destroy(other.gameObject);
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
    public int GetHp()
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
        hp -= _damage;
    }

    #endregion

    #region ----------------------------[Animation]----------------------------

    private void Attack_00_Enter()
    {
        atkrangeList[0].SetActive(true);
        atkrangeList[0].GetComponent<AttackTrigger>().SetDamage(damage);
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

        if (Input.GetKey(KeyCode.LeftShift))
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

        if (Input.GetKeyDown(KeyCode.Space) && itemcount / itemMagnification >= 1)
        {
            itemcount = 0;

            switch (itemcode)
            {
                case 0:
                    GameObject areaskill = Instantiate(resource.GetItemPrefeb((byte)ITEMCODE.CROWN), transform.position, Quaternion.identity);
                    WideAreaSkill wide = areaskill.GetComponent<WideAreaSkill>();
                    wide.SetSkillPreset("Monster", 3f);
                    break;
                case 1:
                    Debug.Log("슬롯머신 발동!");
                    break;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && isdispoitemon)
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

    private void CalculateDirection()
    {
        angle = Mathf.Atan2(input.x, input.y);
        angle = Mathf.Rad2Deg * angle;
        angle += Camera.main.transform.eulerAngles.y;
    }

    private void Rotate()
    {
        targetrotation = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetrotation, turnspeed * Time.deltaTime);
    }

    private void Move()
    {
        anim.SetBool("Run", true);
        transform.position += transform.forward * velocity * Time.deltaTime *  speed;
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
