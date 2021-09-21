using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DungeonMaker : MonoBehaviour
{
    private TileData[,] mapDataArr = new TileData[51, 51];
    private TileInfo[,] mapBoardArr = new TileInfo[51, 51];
    private byte[] monMaxArr = new byte[50];

    private GameObject curEmptyRoom;
    private GameObject curMiniMap;
    private GameObject miniMapGroup;

    private GameObject obFloor;
    private GameObject obBlockWall;
    private GameObject obDoorWall;

    private GameObject mapFloor;
    private GameObject mapBlockWall;
    private GameObject mapDoorWall;

    private Vector2? curTilePos = null;

    private byte StageThemeIndex = 1;

    private int curIndex = -1;

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
    }

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

        LoadMap();
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

                foreach (KeyValuePair<string, MONSTERTYPE> _dicionary in _data.monSpawnInfoDic)
                {
                    string[] _xyPos = _dicionary.Key.Split('/');

                    float _x = float.Parse(_xyPos[0]);
                    float _y = float.Parse(_xyPos[1]);

                    MONSTERTYPE _type = _dicionary.Value;

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

                mapBoardArr[BoardPosParse(x), BoardPosParse(y)] = _info;
            }
        }
    }

    private void LoadMap()
    {
        foreach (TileInfo _info in mapBoardArr)
        {
            if (_info != null)
            {
                curTilePos = _info.position;

                int x = (int)curTilePos.Value.x;
                int y = (int)curTilePos.Value.y;

                TileInfo info = mapBoardArr[BoardPosParse(x), BoardPosParse(y)];

                CreateMap();
                
                foreach (KeyValuePair<Vector2, MONSTERTYPE> _dicionary in _info.monSpawnInfoDic)
                {
                    Vector2 _pos = _dicionary.Key;
                    
                    GameObject _monPrefab = Resources.Load("Prefab/Characters/Monsters/Monster" + (int)_dicionary.Value) as GameObject;

                    GameObject _monster = Instantiate(_monPrefab, new Vector3(_pos.x,1 , _pos.y), Quaternion.identity);
                    
                    _monster.name = string.Format("({0}, {1}){2}", _pos.x, _pos.y, _dicionary.Value);
                    _monster.transform.parent = mapBoardArr[BoardPosParse(x), BoardPosParse(y)].obTile.transform;

                    if((int)_dicionary.Value == 1)
                    {
                        monMaxArr[info.roomIndex]++;
                    }
                }

                foreach (KeyValuePair<Vector2, FurnitureInfo> _dicionary in _info.FurnitureInfoDic)
                {
                    Vector2 _pos = _dicionary.Key;

                    string _name = _dicionary.Value.name;

                    int _rotate = (int)_dicionary.Value.dir * 90;

                    GameObject obfurniture = Resources.Load("Prefab/Furniture/" + _name) as GameObject;

                    GameObject _furniture = Instantiate(obfurniture, new Vector3(_pos.x - 0.5f, 1, _pos.y - 0.5f), Quaternion.Euler(new Vector3(0, _rotate, 0)));
                    _furniture.name = string.Format("{0} // {1} {2}", _pos.x, _pos.y, _name);
                    _furniture.transform.parent = mapBoardArr[BoardPosParse(x), BoardPosParse(y)].obTile.transform;
                }
            }
        }
    }

    public void MonsterDead()
    {
        monMaxArr[curIndex]--;

        Debug.Log(monMaxArr[curIndex]);
    }

    public void MoveNextRoom(Vector2 _nextPos)
    {
        Debug.Log(_nextPos);

        if (monMaxArr[curIndex] == 0)
        {
            GameObject _player = GameObject.Find("Player");
            _player.transform.position = new Vector3(_nextPos.x, 0, _nextPos.y);

            GameObject curroom = GameObject.Find(string.Format("Room {0}", curIndex));

            for (int i = 0; i < curroom.transform.childCount; i++)
            {
                curroom.transform.GetChild(i).gameObject.SetActive(false);
            }

            curIndex = mapBoardArr[BoardPosParse((int)_nextPos.x), BoardPosParse((int)_nextPos.y)].roomIndex;

            GameObject nextroom = GameObject.Find(string.Format("Room {0}", curIndex));

            for (int i = 0; i < nextroom.transform.childCount; i++)
            {
                nextroom.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        else
        {
            Debug.Log("해치우지 못한 몬스터가 남아있습니다.");
        }
    }

    public void CreateMap()
    {
        int _x = (int)curTilePos.Value.x;
        int _y = (int)curTilePos.Value.y;

        TileInfo _GetInfo = mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)];

        if (_GetInfo != null)
        {
            curEmptyRoom = GameObject.Find(string.Format("Room {0}", _GetInfo.roomIndex));

            if (curEmptyRoom == null)
            {
                curEmptyRoom = new GameObject(string.Format("Room {0}", _GetInfo.roomIndex));
            }

            #region[TileObject]

            GameObject emptytile = new GameObject(string.Format("Tile/{0},{1}", (_x / 18), (_y / 18)));

            emptytile.transform.position = new Vector3(_x, 0, _y);
            emptytile.transform.parent = curEmptyRoom.transform;

            GameObject floor = Instantiate(obFloor, new Vector3(_x, 0, _y), Quaternion.identity);
            floor.transform.parent = emptytile.transform;

            #endregion

            #region[MiniMapTile]

            curMiniMap = new GameObject(string.Format("MiniMap/{0},{1}", (_x / 18), (_y / 18)));

            curMiniMap.transform.position = new Vector3(_x, 0, _y);
            curMiniMap.transform.parent = miniMapGroup.transform;

            GameObject mapfloor = Instantiate(mapFloor, new Vector3(_x, 0, _y), Quaternion.identity);       
            mapfloor.transform.parent = curMiniMap.transform;

            #endregion

            mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)].obTile = emptytile;
            mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)].position = new Vector2(_x, _y);

            BuildWall(mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)]);

            floor.GetComponent<NavMeshSurface>().BuildNavMesh();

            if (_GetInfo.roomIndex != 1)        //임시
            {
                emptytile.SetActive(false);
                curIndex = 1;
            }
        }
    }

    private void BuildWall(TileInfo _tileinfo)
    {
        if (_tileinfo.obTile.transform.Find("Walls") != null)
        {
            Destroy(_tileinfo.obTile.transform.Find("Walls").gameObject);
        }

        GameObject emptywall;           //벽들을 자식으로 가지고 있을 빈오브젝트

        emptywall = new GameObject("Walls");
        emptywall.transform.parent = _tileinfo.obTile.transform;

        for (int i = 0; i < 4; i++)
        {
            GameObject _wall = null;
            GameObject _map = null;

            Vector2 _pos = _tileinfo.position;
            int _x = (int)_pos.x;
            int _y = (int)_pos.y;

            switch (_tileinfo.doorArr[i])       //배열에서 벽 확인
            {
                case WALLSTATE.BLOCK:
                    _wall = obBlockWall;
                    _map = mapBlockWall;
                    break;
                case WALLSTATE.EMPTY:
                    break;
                case WALLSTATE.DOOR:
                    _wall = obDoorWall;
                    _map = mapDoorWall;
                    break;
            }

            if (_wall != null)
            {
                switch (i)
                {
                    case (int)DIRECTION.TOP:

                        GameObject topWall = Instantiate(_wall, new Vector3(_x, 4, 8.5f + _y), Quaternion.Euler(0, 90, 90));
                        topWall.name = "Top Wall";
                        topWall.transform.parent = emptywall.transform;

                        GameObject topWallMap = Instantiate(_map, new Vector3(_x, 4, 8.5f + _y), Quaternion.Euler(0, 90, 90));
                        topWallMap.name = "Top Wall Map";
                        topWallMap.transform.parent = curMiniMap.transform;

                        break;
                    case (int)DIRECTION.RIGHT:

                        GameObject rightWall = Instantiate(_wall, new Vector3(_x + 8.5f, 4, _y), Quaternion.Euler(0, -180, 90));
                        rightWall.name = "Right Wall";
                        rightWall.transform.parent = emptywall.transform;

                        GameObject rightWallMap = Instantiate(_map, new Vector3(_x + 8.5f, 4, _y), Quaternion.Euler(0, -180, 90));
                        rightWallMap.name = "Right Wall Map";
                        rightWallMap.transform.parent = curMiniMap.transform;

                        break;
                    case (int)DIRECTION.BOTTOM:

                        GameObject bottomWall = Instantiate(_wall, new Vector3(_x, 4, _y - 8.5f), Quaternion.Euler(0, -90, 90));
                        bottomWall.name = "Bottom Wall";
                        bottomWall.transform.parent = emptywall.transform;

                        GameObject bottomWallMap = Instantiate(_map, new Vector3(_x, 4, _y - 8.5f), Quaternion.Euler(0, -90, 90));
                        bottomWallMap.name = "Bottom Wall Map";
                        bottomWallMap.transform.parent = curMiniMap.transform;

                        break;
                    case (int)DIRECTION.LEFT:

                        GameObject leftWall = Instantiate(_wall, new Vector3(_x - 8.5f, 4, _y), Quaternion.Euler(0, 0, 90));
                        leftWall.name = "Left Wall";
                        leftWall.transform.parent = emptywall.transform;

                        GameObject leftWallMap = Instantiate(_map, new Vector3(_x - 8.5f, 4, _y), Quaternion.Euler(0, 0, 90));
                        leftWallMap.name = "Left Wall Map";
                        leftWallMap.transform.parent = curMiniMap.transform;

                        break;
                }
            }                
        }
    }

    private byte BoardPosParse(int _pos)
    {
        return (byte)(_pos / 18 + 25);
    }
}
