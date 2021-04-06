﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WEAPONID
{
    SWORD,
    SPEAR
}

//[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    private List<GameObject> atkrangeList = new List<GameObject>();

    private GameObject[] itemArr;

    private bool isattack = false;
    private bool ontextzone = false;

    private int damage = 3;
    private int hp = 200;
    private float stamina = 20;
    [SerializeField]
    private float runspeed = 1.5f;
    private float speed = 1f;

    private float itemMagnification = 6;
    [SerializeField]
    private float itemcount = 1;

    private bool isdispoitemon = false;
    private int dispoitemcode = 0;
    private int itemcode = 0;

    private Animator anim;

    private GameObject obzone;

    public WEAPONID WeaponID = WEAPONID.SWORD;

    private float velocity = 5f;
    private float turnspeed = 10f;

    private Vector2 input;
    private float angle;

    private Quaternion targetrotation;

    void Start()
    {
        itemArr = Resources.LoadAll<GameObject>("Prefab/Item");

        anim = gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        if (ontextzone)
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

    public void SetItemMagnification(int _magnification)
    {
        itemMagnification = _magnification;
    }

    public float CheckItemCount()
    {
        return itemcount / itemMagnification;
    }

    public void GetAtkRangList(GameObject _atkrang)
    {
        atkrangeList.Add(_atkrang);
    }

    public float CheckStamina()
    {
        return stamina;
    }
    public bool OnTextZone()
    {
        return ontextzone;
    }

    public void ExitTextZone()
    {
        ontextzone = false;
    }

    public GameObject ObZone()
    {
        return obzone;
    }

    public void GetDamage(int _damage)
    {
        hp -= _damage;
    }

    public int CheckHp()
    {
        return hp;
    }

    public int CheckItemCode()
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
                    Instantiate(itemArr[0], transform.position, Quaternion.identity);
                    //itemArr[0];
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
