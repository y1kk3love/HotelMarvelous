using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMove : MonoBehaviour
{
    private Animator anim;
    private DungeonPathFinder pathfinder;

    private GameObject obPlayer;

    private List<Tile> pathList = new List<Tile>();

    private Vector3 targetPos;

    public bool[,] tileMapArr;
    private bool isfindtarget = false;
    public bool isDead = false;

    private int pathcount = 0;

    private float movedelay = 0;
    private float targetPathDis;
    public float speed = 5;    

    void Start()
    {
        anim = transform.GetComponent<Animator>();
        pathfinder = GameObject.Find("DungeonManager").transform.GetComponent<DungeonPathFinder>();
    }

    void Update()
    {
        if(obPlayer == null)
        {
            obPlayer = GameObject.FindGameObjectWithTag("Player");

            if (obPlayer != null) 
            {
                targetPos = obPlayer.transform.position;
            }
        }

        Move();
    }

    private void Move()
    {
        if (isDead || ScenesManager.Instance.CheckScene() == "MapTools")
        {
            return;
        }

        FindPlayer();

        MoveToPath();
    }

    private void MoveToPath()
    {
        Vector3 dir = new Vector3(0, 0, 0);

        targetPathDis = Vector3.Distance(transform.position, GetNextPath()); 

        if(targetPathDis > 1f)
        {
            dir = (GetNextPath() - transform.position).normalized;
        }
        else if(targetPathDis > 0)
        {
            pathcount++;
            dir = (GetNextPath() - transform.position).normalized;
        }

        transform.position += Time.deltaTime * dir * speed;

        if(anim != null)
        {
            if (dir == new Vector3(0, 0, 0))
            {
                anim.SetBool("Move", false);
            }
            else
            {
                transform.forward = dir;
                anim.SetBool("Move", true);
            }
        }
    }

    private void FindPlayer()
    {
        movedelay += Time.deltaTime;

        if(movedelay >= 1f && !isfindtarget)
        {
            movedelay = 0;
            isfindtarget = true;

            pathList.Clear();
            pathcount = 0;
            pathList = pathfinder.PathFind(CurrentPosition(), PlayerPosition());
        }

        if(movedelay >= 5)
        {
            movedelay = 0;
            isfindtarget = false;
        }
    }

    private Vector3 GetNextPath()
    {
        if (pathList != null && pathcount < pathList.Count)
        {
            float x = pathList[pathcount].X - 8.5f;
            float y = pathList[pathcount].Y - 8.5f;

            return new Vector3(x, 1, y);
        }

        return transform.position;
    }

    private Tile CurrentPosition()
    {
        float x = Mathf.Floor(transform.position.x);
        float y = Mathf.Floor(transform.position.z);

        if (x < 0) { x -= 0.5f; }
        if (x > 0) { x += 0.5f; }
        if (y < 0) { y -= 0.5f; }
        if (y > 0) { y += 0.5f; }

        Tile start = new Tile(false, (int)(x + 8.5f), (int)(y + 8.5f));

        return start;
    }

    private Tile PlayerPosition()
    {
        if(obPlayer != null)
        {
            float x = Mathf.Floor(obPlayer.transform.position.x);
            float y = Mathf.Floor(obPlayer.transform.position.z);

            if (x < 0) { x -= 0.5f; }
            if (x > 0) { x += 0.5f; }
            if (y < 0) { y -= 0.5f; }
            if (y > 0) { y += 0.5f; }

            Tile target = new Tile(false, (int)(x + 8.5f), (int)(y + 8.5f));

            return target;
        }
        else
        {
            return null;
        }
    }

    public void SetTargetPos(Vector3 _pos)
    {
        targetPos = _pos;
    }
}
