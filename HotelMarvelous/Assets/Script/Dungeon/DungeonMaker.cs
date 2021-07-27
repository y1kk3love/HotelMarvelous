using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DungeonMaker : MonoBehaviour
{
    private List<Vector2> selectTilesList = new List<Vector2>();

    private TileData[,] mapDataArr = new TileData[51, 51];
    private TileInfo[,] mapBoardArr = new TileInfo[51, 51];

    private GameObject obFloor;
    private GameObject obBlockWall;
    private GameObject obDoorWall;

    private int floor;

    void Start()
    {
        obFloor = Resources.Load("Prefab/Stage/Floor" + floor) as GameObject;
    }

    void Update()
    {

    }

    public void LoadData()
    {
        string filename = string.Format(@"{0}/Stage/{1}F_{2}.map", Application.streamingAssetsPath, floor, Random.Range(0, 4));

        BinaryFormatter bf = new BinaryFormatter();

        FileStream fs = File.Open(filename, FileMode.Open);

        if (fs != null && fs.Length > 0)
        {
            mapDataArr = (TileData[,])bf.Deserialize(fs);

            fs.Close();
        }

        GetDataParse();

        foreach (TileInfo _info in mapBoardArr)
        {
            if (_info != null)
            {
                Vector2 pos = _info.position;
                int x = (int)pos.x;
                int y = (int)pos.y;

                selectTilesList.Clear();

                selectTilesList.Add(pos);

                CreateMap();

                foreach (KeyValuePair<Vector2, MONSTERTYPE> _dicionary in _info.monSpawnInfoDic)
                {
                    Vector2 _pos = _dicionary.Key;
                    
                    GameObject _monPrefab = Resources.Load("Prefab/Characters/Monsters/" + (int)_dicionary.Value) as GameObject;

                    //위치 수정 필요!!
                    GameObject _monster = Instantiate(_monPrefab, new Vector2(_pos.x - 0.5f, _pos.y - 0.5f), Quaternion.identity);
                    //이름 수정 필요!!
                    _monster.name = string.Format("{0} // {1} MonPos", _pos.x, _pos.y);
                    _monster.transform.parent = mapBoardArr[x + 25, y + 25].obTile.transform;
                }

                foreach (KeyValuePair<Vector2, FurnitureInfo> _dicionary in _info.FurnitureInfoDic)
                {
                    Vector2 _pos = _dicionary.Key;

                    string _name = _dicionary.Value.name;

                    int _rotate = (int)_dicionary.Value.dir * 90;

                    GameObject obfurniture = Resources.Load("Prefab/Furniture/" + _name) as GameObject;

                    //위치 수정 필요!!
                    GameObject _furniture = Instantiate(obfurniture, new Vector2(_pos.x - 0.5f, _pos.y - 0.5f), Quaternion.Euler(new Vector3(-90 + _rotate, 90, -90)));
                    //이름 수정 필요!!
                    _furniture.name = string.Format("{0} // {1} {2}", _pos.x, _pos.y, _name);
                    _furniture.transform.parent = mapBoardArr[x + 25, y + 25].obTile.transform;
                }
            }
        }
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

                mapBoardArr[x + 25, y + 25] = _info;
            }
        }
    }

    public void CreateMap()
    {
        foreach (Vector2 _pos in selectTilesList)
        {
            int _x = (int)_pos.x;
            int _y = (int)_pos.y;

            if (mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)] == null)
            {
                //이름 수정 필요
                GameObject emptytile = new GameObject(string.Format("Tile/{0},{1}", (_x / 18), (_y / 18)));
                //위치 수정 필요
                emptytile.transform.position = new Vector3(_x, _y, 9);

                //위치 수정 필요
                GameObject floor = Instantiate(obFloor, new Vector3(_x, _y, 9), Quaternion.identity);
                floor.transform.parent = emptytile.transform;

                mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)].obTile = emptytile;
                mapBoardArr[BoardPosParse(_x), BoardPosParse(_y)].position = new Vector3(_x, _y, 9);
            }
        }

        foreach (Vector2 _pos in selectTilesList)
        {
            BuildWall(mapBoardArr[BoardPosParse((int)_pos.x), BoardPosParse((int)_pos.y)]);       //생성된 타일 위에 벽 생성
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

            Vector2 _pos = _tileinfo.obTile.transform.position;
            int _x = (int)_pos.x;
            int _y = (int)_pos.y;

            switch (_tileinfo.doorArr[i])       //배열에서 벽 확인
            {
                case WALLSTATE.BLOCK:
                    _wall = obBlockWall;
                    break;
                case WALLSTATE.EMPTY:
                    break;
                case WALLSTATE.DOOR:
                    _wall = obDoorWall;
                    break;
            }

            if (_wall != null)
            {
                switch (i)
                {
                    case (int)DIRECTION.TOP:

                        GameObject topWall = Instantiate(_wall, new Vector2(_x, _y + 8.5f), Quaternion.Euler(0, 0, 90));
                        topWall.name = "TopWall";
                        topWall.transform.parent = emptywall.transform;

                        break;
                    case (int)DIRECTION.RIGHT:

                        GameObject rightWall = Instantiate(_wall, new Vector2(_x + 8.5f, _y), Quaternion.Euler(0, 0, 0));
                        rightWall.name = "RightWall";
                        rightWall.transform.parent = emptywall.transform;

                        break;
                    case (int)DIRECTION.BOTTOM:

                        GameObject bottomWall = Instantiate(_wall, new Vector2(_x, _y - 8.5f), Quaternion.Euler(0, 0, -90));
                        bottomWall.name = "BottomWall";
                        bottomWall.transform.parent = emptywall.transform;

                        break;
                    case (int)DIRECTION.LEFT:

                        GameObject leftWall = Instantiate(_wall, new Vector2(_x - 8.5f, _y), Quaternion.Euler(0, 0, -180));
                        leftWall.name = "LeftWall";
                        leftWall.transform.parent = emptywall.transform;

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
