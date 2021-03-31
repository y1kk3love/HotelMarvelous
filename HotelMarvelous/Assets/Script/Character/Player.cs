using System.Collections;
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

    private bool isattack = false;
    private bool ontextzone = false;

    private int hp = 200;
    private float stamina = 20;
    private float runspeed = 1.5f;

    private Animator anim;

    private GameObject obzone;

    public WEAPONID WeaponID = WEAPONID.SWORD;

    private float velocity = 5f;
    private float turnspeed = 10f;

    private Vector2 input;
    private float angle;

    private Quaternion targetrotation;
    private Transform cam;

    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        cam = Camera.main.transform;
    }

    void Update()
    {
        if (ontextzone)
        {
            anim.SetBool("Run", false);
            return;
        }

        GetInput();

        if (!isattack)
        {
            Attack();

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

    #endregion

    #region ----------------------------[Animation]----------------------------

    private void AttackEnter()
    {
        Debug.Log("공격시작");
    }

    private void AttackExit()
    {
        Debug.Log("공격끝");
    }

    #endregion

    #region ----------------------------[PlayerControl]----------------------------

    private void GetInput()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
    }

    private void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            StartCoroutine(Attack_01());
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if(stamina > 0)
            {
                stamina -= Time.deltaTime;
                runspeed = 10f;
            }
            else
            {
                stamina = 0;
                runspeed = 1;
            }
        }
        else
        {
            runspeed = 1;

            if(stamina <= 20)
            {
                stamina += Time.deltaTime / 3;
            }
            else
            {
                stamina = 20;
            }
        }
    }

    private void CalculateDirection()
    {
        angle = Mathf.Atan2(input.x, input.y);
        angle = Mathf.Rad2Deg * angle;
        angle += cam.eulerAngles.y;
    }

    private void Rotate()
    {
        targetrotation = Quaternion.Euler(0, angle, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetrotation, turnspeed * Time.deltaTime);
    }

    private void Move()
    {
        anim.SetBool("Run", true);
        transform.position += transform.forward * velocity * Time.deltaTime *  runspeed;
    }

    IEnumerator Attack_01()
    {
        anim.SetBool("Attack_01", true);
        atkrangeList[0].SetActive(true);
        isattack = true;

        yield return new WaitForSeconds(1.02f);

        anim.SetBool("Attack_01", false);
        atkrangeList[0].SetActive(false);
        isattack = false;
    }

    #endregion
}
