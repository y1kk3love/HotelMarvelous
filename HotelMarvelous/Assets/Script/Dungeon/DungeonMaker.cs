using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DungeonMaker : MonoBehaviour
{
    private List<TileInfo>[] roomIndexListArr = new List<TileInfo>[20];

    private TileData[,] mapDataArr = new TileData[51, 51];

    private byte[,] boardRoomIndexArr = new byte[51, 51];
    private byte[] monMaxArr = new byte[50];

    private bool[] isRoomLoadArr = new bool[50];

    private Player player;

    public GameObject blackOut;
    public GameObject bossUI;
    public GameObject rewardItem;

    private GameObject curEmptyRoom;
    private GameObject curMiniMap;
    private GameObject miniMapGroup;

    private GameObject obFloor;
    private GameObject obBlockWall;
    private GameObject obDoorWall;

    private GameObject mapFloor;
    private GameObject mapBlockWall;
    private GameObject mapDoorWall;

    private Vector2 curTilePos;

    private byte StageThemeIndex = 1;

    private int curIndex = 1;

    void Start()
    {
        obFloor = Resources.Load("Prefab/Stage/Floor_" + StageThemeIndex) as GameObject;
        obBlockWall = Resources.Load("Prefab/Stage/Wall_" + StageThemeIndex) as GameObject;
        obDoorWall = Resources.Load("Prefab/Stage/Door_" + StageThemeIndex) as GameObject;

        mapFloor = Resources.Load("Prefab/Stage/MiniMap/Floor") as GameObject;
        mapBlockWall = Resources.Load("Prefab/Stage/MiniMap/Wall") as GameObject;
        mapDoorWall = Resources.Load("Prefab/Stage/MiniMap/Door") as GameObject;

        miniMapGroup = new GameObject("MiniMaps");

        LoadData();

        LoadMap(1);

        RoomEnd();

        GameObject prefab = Resources.Load("Prefab/Characters/PC/Player") as GameObject;
        player = Instantiate(prefab, new Vector3(3, 0.5f, 3), Quaternion.identity).GetComponent<Player>();
        player.gameObject.name = "Player";

        GameObject _ui = Instantiate(blackOut);
        Destroy(_ui, 2f);
    }

    public void MonsterDead()
    {
        monMaxArr[curIndex]--;

        RoomEnd();
    }

    public void MonsterAdd()
    {
        monMaxArr[curIndex]++;
    }

    private void RoomReward()
    {
        int random = Random.Range(1, 100);

        if(random < 100)
        {
            int middle = (int)System.Math.Truncate((double)(roomIndexListArr[curIndex].Count / 2));

            TileInfo info = roomIndexListArr[curIndex][middle];

            if (info.roomType == ROOMTYPE.HALLWAY)
            {
                GameObject obj = Instantiate(rewardItem, new Vector3(info.position.x, 0.5f, info.position.y), Quaternion.identity);

                RewardItem reward = obj.transform.GetComponent<RewardItem>();
                reward.transform.SetParent(roomIndexListArr[curIndex][0].obTile.transform);
                reward.id = (CONSUMITEM)Random.Range(1, System.Enum.GetValues(typeof(CONSUMITEM)).Length + 1);
            }
        }
    }

    public bool RoomClear()
    {
        if (monMaxArr[curIndex] == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private IEnumerator RoomStart()
    {
        GameObject[] Door = GameObject.FindGameObjectsWithTag("Door");

        for (int i = 0; i < Door.Length; i++)
        {
            Door[i].GetComponent<Animator>().SetBool("isDoorOpen", true);
        }

        yield return new WaitForSeconds(2f);

        for (int i = 0; i < Door.Length; i++)
        {
            Door[i].GetComponent<Animator>().SetBool("isDoorOpen", false);
        }
    }

    private void RoomEnd()
    {
        if(monMaxArr[curIndex] == 0)
        {
            GameObject[] Door = GameObject.FindGameObjectsWithTag("Door");

            for (int i = 0; i < Door.Length; i++)
            {
                Door[i].GetComponent<Animator>().SetBool("isDoorOpen", true);
            }

            RoomReward();
        }

        transform.GetComponent<DungeonPathFinder>().ResetGridMap(roomIndexListArr[curIndex]);
    }

    public void MoveNextRoom(Vector2 _nextPos, Vector2 _dir)
    {
        if (monMaxArr[curIndex] == 0)
        {
            GameObject _ui = Instantiate(blackOut);
            Destroy(_ui, 2f);

            StartCoroutine(MovePlayer(_dir / 6));

            for (int i = 0; i < roomIndexListArr[curIndex].Count; i++)
            {
                roomIndexListArr[curIndex][i].obTile.SetActive(false);
            }

            curIndex = boardRoomIndexArr[BoardPosParse((int)_nextPos.x), BoardPosParse((int)_nextPos.y)];

            if (!isRoomLoadArr[curIndex])
            {
                LoadMap(curIndex);

                if(roomIndexListArr[curIndex][0].roomType == ROOMTYPE.BOSS)
                {
                    GameObject bossui = Instantiate(bossUI);
                    bossui.name = "BossUI";
                }
            }
            else
            {
                for (int i = 0; i < roomIndexListArr[curIndex].Count; i++)
                {
                    roomIndexListArr[curIndex][i].obTile.SetActive(true);
                }
            }

            if (monMaxArr[curIndex] == 0)
            {
                RoomEnd();
            }
            else
            {
                StartCoroutine(RoomStart());
            }
        }
        else
        {
            GameObject[] remainMonster = GameObject.FindGameObjectsWithTag("Monster");
            GameObject boss = GameObject.FindGameObjectWithTag("Boss");

            if (remainMonster.Length == 0 && boss == null)
            {
                monMaxArr[curIndex] = 0;
                RoomEnd();
            }
            else
            {
                Debug.Log("해치우지 못한 몬스터가 남아있습니다.");
            }
        }
    }

    IEnumerator MovePlayer(Vector2 _pos)
    {
        player.stopAllMove = true;
        player.transform.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.5f);

        player.transform.gameObject.SetActive(true);
        player.transform.forward = new Vector3(_pos.x, 0, _pos.y);
        player.transform.position += new Vector3(_pos.x, 0, _pos.y);

        StartCoroutine(player.MoveInIntro(1f, (new Vector3(_pos.x, 0, _pos.y)).normalized));
    }

    #region [DATALOAD]

    public void LoadData()
    {
        string filename = string.Format(@"{0}/Stage/{1}F_{2}.map", Application.streamingAssetsPath, StageThemeIndex, Random.Range(1, 1));

        BinaryFormatter bf = new BinaryFormatter();

        FileStream fs = File.Open(filename, FileMode.Open);

        if (fs != null && fs.Length > 0)
        {
            mapDataArr = (TileData[,])bf.Deserialize(fs);

            fs.Close();
        }

        GetDataParse();
    }

    private void GetDataParse()
    {
        foreach (TileData _data in mapDataArr)
        {
            if (_data != null)
            {
                TileInfo _info = new TileInfo();

                string[] xyPos = _data.pos.Split('/');
                int x = int.Parse(xyPos[0]);
                int y = int.Parse(xyPos[1]);

                _info.position = new Vector2(x, y);
                _info.roomIndex = _data.roomIndex;
                _info.roomType = _data.roomType;
                _info.doorArr = _data.doorArr;

                foreach (KeyValuePair<string, int> _dicionary in _data.monSpawnInfoDic)
                {
                    string[] _xyPos = _dicionary.Key.Split('/');

                    float _x = float.Parse(_xyPos[0]);
                    float _y = float.Parse(_xyPos[1]);

                    int _type = _dicionary.Value;

                    _info.monSpawnInfoDic.Add(new Vector2(_x, _y), _type);
                }

                foreach (KeyValuePair<string, FurnitureData> _dicionary in _data.FurnitureInfoDic)
                {
                    FurnitureInfo _finfo = new FurnitureInfo();

                    string[] _xyPos = _dicionary.Key.Split('/');

                    float _x = float.Parse(_xyPos[0]);
                    float _y = float.Parse(_xyPos[1]);

                    FurnitureData _fdata = _dicionary.Value;

                    _finfo.name = _fdata.name;
                    _finfo.pos = new Vector2(_x, _y);
                    _finfo.dir = _fdata.dir;

                    _info.FurnitureInfoDic.Add(_finfo.pos, _finfo);
                }

                if(roomIndexListArr[_info.roomIndex] == null)
                {
                    List<TileInfo> roomIndexList = new List<TileInfo>();

                    roomIndexListArr[_info.roomIndex] = roomIndexList;
                }

                roomIndexListArr[_info.roomIndex].Add(_info);

                boardRoomIndexArr[BoardPosParse(x), BoardPosParse(y)] = _info.roomIndex;

                CreateMiniMap(_info);
            }
        }

        mapDataArr = null;
    }

    #endregion

    #region [CreateRoom]

    private void LoadMap(int roomindex)
    {
        for (int i = 0; i < roomIndexListArr[roomindex].Count; i++)
        {
            TileInfo _info = roomIndexListArr[roomindex][i];

            curTilePos = _info.position;

            CreateMap(_info);

            foreach (KeyValuePair<Vector2, int> _dicionary in _info.monSpawnInfoDic)
            {
                Vector2 _pos = _dicionary.Key;

                string name = ResourceManager.Instance.GetObjectName(OBJECTDATA.MONSTERNAME, _dicionary.Value);

                Debug.Log(name);

                GameObject _monPrefab = Resources.Load("Prefab/Characters/Monsters/" + name) as GameObject;

                GameObject _monster = Instantiate(_monPrefab, new Vector3(_pos.x, 1, _pos.y), Quaternion.identity);

                _monster.name = string.Format("({0}, {1}){2}", _pos.x, _pos.y, _dicionary.Value);
                _monster.transform.parent = _info.obTile.transform;

                monMaxArr[_info.roomIndex]++;
            }

            foreach (KeyValuePair<Vector2, FurnitureInfo> _dicionary in _info.FurnitureInfoDic)
            {
                Vector2 _pos = _dicionary.Key;

                string _name = _dicionary.Value.name;

                int _rotate = (int)_dicionary.Value.dir * 90;

                GameObject obfurniture = Resources.Load("Prefab/Furniture/" + _name) as GameObject;

                GameObject _furniture = Instantiate(obfurniture, new Vector3(_pos.x - 0.5f, 1, _pos.y - 0.5f), Quaternion.Euler(new Vector3(0, _rotate, 0)));
                _furniture.name = string.Format("{0} // {1} {2}", _pos.x, _pos.y, _name);
                _furniture.transform.parent = _info.obTile.transform;
            }
        }
    }

    #region [MiniMap]
    public void CreateMiniMap(TileInfo _tileinfo)
    {
        int _x = (int)_tileinfo.position.x;
        int _y = (int)_tileinfo.position.y;

        curMiniMap = new GameObject(string.Format("MiniMap/{0},{1}", (_x / 18), (_y / 18)));

        curMiniMap.transform.position = new Vector3(_x, 0, _y);
        curMiniMap.transform.parent = miniMapGroup.transform;

        GameObject mapfloor = Instantiate(mapFloor, new Vector3(_x, 0, _y), Quaternion.identity);
        mapfloor.transform.parent = curMiniMap.transform;

        BuildMiniMapWall(_tileinfo);
    }

    private void BuildMiniMapWall(TileInfo _tileinfo)
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject _map = null;

            Vector2 _pos = _tileinfo.position;
            int _x = (int)_pos.x;
            int _y = (int)_pos.y;

            switch (_tileinfo.doorArr[i])       //배열에서 벽 확인
            {
                case WALLSTATE.BLOCK:
                    _map = mapBlockWall;
                    break;
                case WALLSTATE.EMPTY:
                    break;
                case WALLSTATE.DOOR:
                    _map = mapDoorWall;
                    break;
            }

            if (_map != null)
            {
                switch (i)
                {
                    case (int)DIRECTION.TOP:

                        GameObject topWallMap = Instantiate(_map, new Vector3(_x, 4, 8.5f + _y), Quaternion.Euler(0, 90, 90));
                        topWallMap.name = "Top Wall Map";
                        topWallMap.transform.parent = curMiniMap.transform;

                        break;
                    case (int)DIRECTION.RIGHT:

                        GameObject rightWallMap = Instantiate(_map, new Vector3(_x + 8.5f, 4, _y), Quaternion.Euler(0, -180, 90));
                        rightWallMap.name = "Right Wall Map";
                        rightWallMap.transform.parent = curMiniMap.transform;

                        break;
                    case (int)DIRECTION.BOTTOM:

                        GameObject bottomWallMap = Instantiate(_map, new Vector3(_x, 4, _y - 8.5f), Quaternion.Euler(0, -90, 90));
                        bottomWallMap.name = "Bottom Wall Map";
                        bottomWallMap.transform.parent = curMiniMap.transform;

                        break;
                    case (int)DIRECTION.LEFT:

                        GameObject leftWallMap = Instantiate(_map, new Vector3(_x - 8.5f, 4, _y), Quaternion.Euler(0, 0, 90));
                        leftWallMap.name = "Left Wall Map";
                        leftWallMap.transform.parent = curMiniMap.transform;

                        break;
                }
            }
        }
    }

    #endregion

    #region [Room]
    public void CreateMap(TileInfo _info)
    {
        int _x = (int)curTilePos.x;
        int _y = (int)curTilePos.y;

        isRoomLoadArr[curIndex] = true;

        curEmptyRoom = GameObject.Find(string.Format("Room {0}", curIndex));

        if (curEmptyRoom == null)
        {
            curEmptyRoom = new GameObject(string.Format("Room {0}", curIndex));
        }

        #region[TileObject]

        GameObject emptytile = new GameObject(string.Format("Tile/{0},{1}", (_x / 18), (_y / 18)));

        emptytile.transform.position = new Vector3(_x, 0, _y);
        emptytile.transform.parent = curEmptyRoom.transform;

        GameObject floor = Instantiate(obFloor, new Vector3(_x, 0, _y), Quaternion.identity);
        floor.transform.parent = emptytile.transform;

        #endregion

        _info.obTile = emptytile;
        _info.position = new Vector2(_x, _y);

        BuildWall(_info);

        //로딩 렉의 원인!!
        //floor.GetComponent<NavMeshSurface>().BuildNavMesh();
    }

    private void BuildWall(TileInfo _tileinfo)
    {
        if (_tileinfo.obTile.transform.Find("Walls") != null)
        {
            Destroy(_tileinfo.obTile.transform.Find("Walls").gameObject);
        }

        GameObject emptywall = new GameObject("Walls");           //벽들을 자식으로 가지고 있을 빈오브젝트
        emptywall.transform.parent = _tileinfo.obTile.transform;

        for (int i = 0; i < 4; i++)
        {
            GameObject _wallPrefab = null;

            Vector2 _pos = _tileinfo.position;
            int _x = (int)_pos.x;
            int _y = (int)_pos.y;

            WALLSTATE wallDir = _tileinfo.doorArr[i];

            switch (wallDir)       //배열에서 벽 확인
            {
                case WALLSTATE.BLOCK:
                    _wallPrefab = obBlockWall;
                    break;
                case WALLSTATE.EMPTY:
                    break;
                case WALLSTATE.DOOR:
                    _wallPrefab = obDoorWall;
                    break;
            }

            GameObject Wall = null;

            if (_wallPrefab != null)
            {
                switch (i)
                {
                    case (int)DIRECTION.TOP:

                        Wall = Instantiate(_wallPrefab, new Vector3(_x, 4, 8.5f + _y), Quaternion.Euler(0, 90, 90));
                        break;
                    case (int)DIRECTION.RIGHT:

                        Wall = Instantiate(_wallPrefab, new Vector3(_x + 8.5f, 4, _y), Quaternion.Euler(0, -180, 90));
                        break;
                    case (int)DIRECTION.BOTTOM:

                        Wall = Instantiate(_wallPrefab, new Vector3(_x, 4, _y - 8.5f), Quaternion.Euler(0, -90, 90));
                        break;
                    case (int)DIRECTION.LEFT:

                        Wall = Instantiate(_wallPrefab, new Vector3(_x - 8.5f, 4, _y), Quaternion.Euler(0, 0, 90));
                        break;
                }

                Wall.name = string.Format("{0} Wall", wallDir.ToString());
                Wall.transform.parent = emptywall.transform;
            }
            
            if(wallDir == WALLSTATE.DOOR)
            {
                DungeonMover door = Wall.GetComponent<DungeonMover>();
                door.doorDir = (DIRECTION)i;
                door.x = (byte)(_x / 18);
                door.y = (byte)(_y / 18);
                door.manager = GetComponent<DungeonMaker>();
            }
        }
    }

    #endregion

    #endregion

    private byte BoardPosParse(int _pos)
    {
        return (byte)(_pos / 18 + 25);
    }
}
