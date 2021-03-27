using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{
    private bool isattack = false;
    private bool ontextzone = false;

    private float velocity = 5f;
    private float turnspeed = 10f;

    private Vector2 input;
    private float angle;

    private Quaternion targetrotation;
    private Transform cam;

    private Animator anim;

    private GameObject obzone;

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
        Attack();

        if (Mathf.Abs(input.x)< 1 && Mathf.Abs(input.y) < 1)
        {
            anim.SetBool("Run", false);
            return;
        }
        if (isattack)
        {
            return;
        }

        CalculateDirection();
        Rotate();
        Move();
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
        transform.position += transform.forward * velocity * Time.deltaTime;
    }

    IEnumerator Attack_01()
    {
        anim.SetBool("Attack_01", true);
        isattack = true;

        yield return new WaitForSeconds(1.02f);

        anim.SetBool("Attack_01", false);
        isattack = false;
    }

    #endregion
}
