using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    private GameObject player;

    private NavMeshAgent navi;

    public int monsterid;

    private int touchdamage;
    [SerializeField]
    private int monsterhp;
    [SerializeField]
    private float monspeed = 1;
    [SerializeField]
    private float Movedelay = 0.5f;
    [SerializeField]
    private float raylength = 5;

    void Start()
    {
        navi = GetComponent<NavMeshAgent>();

        player = GameObject.FindGameObjectWithTag("Player");

        switch (monsterid)
        {
            case 1:
                touchdamage = 10;
                monsterhp = 10;
                break;
        }
    }
    void Update()
    {
        navi.speed = monspeed * 5;

        if(monsterhp <= 0)
        {
            Destroy(gameObject);
        }

        Movedelay -= Time.deltaTime;

        if (Movedelay < 0.0f)
        {
            MoveTargetPlayer();

            Movedelay = Random.Range(0.3f, 1);      //초기화할때 랜덤으로
        }

        Vector3 Debugdir = player.transform.position - transform.position;      //디버깅용
        Debugdir.Normalize();
        Debug.DrawRay(this.transform.position, Debugdir * raylength, Color.red);
    }

    public void MonGetDamage(int _damage)
    {
        Debug.Log("Get " + _damage + "Damage");
        monsterhp -= _damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();
            player.SetDamage(touchdamage);
            Debug.Log(player.GetHp());
        }
    }

    void MoveTargetPlayer()
    {
        Vector3 Monpos = new Vector3(this.transform.position.x, this.transform.position.y + 1.5f, this.transform.position.z);

        RaycastHit Hit;
        Vector3 dir = player.transform.position - Monpos;           //방향값은 목표vector좌표에서 내 vector좌표를 뺀값
        Physics.Raycast(Monpos, dir, out Hit, raylength);

        if (Physics.Raycast(Monpos, dir, out Hit, raylength))
        {
            if (Hit.transform == player.transform)
            {
                navi.SetDestination(Hit.transform.position);
            }
        }
    }
}
