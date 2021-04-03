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

    private int damage = 3;
    private int hp = 200;
    private float stamina = 20;
    [SerializeField]
    private float runspeed = 1.5f;
    private float speed = 1f;

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
        anim = gameObject.GetComponent<Animator>();
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

    private void GetInput()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                Vector3 targetPos = hit.point;
                Vector3 dir = targetPos - transform.position;
                transform.forward = dir.normalized;
            }
        }
    }

    private void Attack()
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
