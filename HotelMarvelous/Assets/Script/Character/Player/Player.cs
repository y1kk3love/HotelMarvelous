﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerStatus stat = DataManager.Instance.GetPlayerStatus();

    private GameObject[] attackRangeArr = new GameObject[3];

    private GameObject curItemSkill = null;

    private bool isInvincible = false;

    #region [Movement]

    private bool isattack = false;
    private bool isconv = false;

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
        attackRangeArr[0] = transform.Find("Sword").GetChild(0).gameObject;

        GetItemInfo();
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
            //ontextzone = true;
            //obzone = other.gameObject;
            Debug.Log("대화 장소에 입장");
        }

        if (other.GetComponent<RewardItem>())
        {
            ResourceManager.Instance.GetItemResource(stat.curItemIndex);
        }

        if (other.gameObject.tag == "ConsumItem")
        {
            CheckDropItem(other);
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
            //ontextzone = false;
            Debug.Log("나가요~");
        }
    }

    #endregion

    #region ----------------------------[Test]----------------------------

    public void Test_AddItemCount()
    {
        stat.curItemStack++;
    }

    public void Test_AddDisoItem()
    {
        stat.curDispoItemIndex = 1;
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
        attackRangeArr[0].SetActive(true);
    }

    private void Attack_00_Exit()
    {
        attackRangeArr[0].SetActive(false);
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

        if (Input.GetKeyDown(ScenesManager.Instance.optionInfo.recharge) && stat.curItemStack / stat.curItemMax >= 1)
        {
            stat.curItemStack = 0;

            switch (stat.curItemIndex)
            {
                case 1:
                    GameObject areaskill = Instantiate(curItemSkill, transform.position, Quaternion.identity);
                    WideAreaSkill wide = areaskill.GetComponent<WideAreaSkill>();
                    wide.SetSkillPreset("Monster", 3f);
                    break;
                case 2:
                    Debug.Log("슬롯머신 발동!");
                    break;
            }
        }

        if (Input.GetKeyDown(ScenesManager.Instance.optionInfo.disposable) && stat.curDispoItemIndex != 255)
        {
            if (stat.curItemStack / stat.curItemMax != 1)
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

    #endregion
}
