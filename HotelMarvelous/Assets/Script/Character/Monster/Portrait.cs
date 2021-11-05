using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portrait : MonoBehaviour
{
    private DungeonMaker dungeon;

    private Animator anim;

    public GameObject dart;

    private float timer = 3;

    private bool isdead = false;
    private bool isAttacking = false;

    void Start()
    {
        anim = transform.GetComponent<Animator>();
        dungeon = GameObject.Find("DungeonMaker").GetComponent<DungeonMaker>();
    }

    void Update()
    {
        if (dungeon != null && dungeon.RoomClear())
        {
            if(!isdead)
            {
                anim.SetTrigger("Dead");

                isdead = true;
            }
            return;
        }

        TargetFoward();

        if (timer >= 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            isAttacking = false;
        }
    }

    private void TargetFoward()
    {
        Vector3 Monpos = new Vector3(transform.position.x, transform.position.y - 1.5f, transform.position.z);

        Debug.DrawRay(Monpos, transform.forward * 10, Color.red);

        RaycastHit Hit;
        if (Physics.Raycast(Monpos, transform.forward, out Hit))
        {
            if (Hit.transform.CompareTag("Player") && !isAttacking)
            {
                anim.SetTrigger("Attack");

                timer = 3;
                isAttacking = true;

                Vector3 startPos = transform.position + new Vector3(0, transform.position.y, 0);
                Vector3 targetPos = Hit.transform.position + new Vector3(0, 2.5f, 0);

                Vector3 v1 = -(startPos - new Vector3(startPos.x, 0, startPos.z)).normalized;
                Vector3 v2 = (targetPos - startPos).normalized;

                float rot = Vector3.Dot(v1, v2);

                Debug.Log(transform.parent.transform.eulerAngles);

                GameObject _dart = Instantiate(dart, startPos, Quaternion.Euler(rot * 90, transform.parent.transform.eulerAngles.y, 0));
                _dart.transform.GetComponent<Projectile>().targetPos = targetPos - startPos;
            }
        }
    }
}
