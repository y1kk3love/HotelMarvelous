using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DungeonMaker : MonoBehaviour
{
    private int floor;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void LoadData()
    {
        string filename = string.Format(@"{0}/Stage/{1}F_{2}.map", Application.streamingAssetsPath, floor, Random.Range(0, 4));
        editingfilename = inputFDataFileName.text;

        BinaryFormatter bf = new BinaryFormatter();

        FileStream fs = File.Open(filename, FileMode.Open);

        if (fs != null && fs.Length > 0)
        {
            mapDataArr = (TileData[,])bf.Deserialize(fs);

            fs.Close();
        }

        isLoadingData = true;
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

                    GameObject _monster = Instantiate(obMonPosCircle, new Vector2(_pos.x - 0.5f, _pos.y - 0.5f), Quaternion.identity);
                    _monster.name = string.Format("{0} // {1} MonPos", _pos.x, _pos.y);
                    _monster.transform.parent = mapBoardArr[x + 25, y + 25].obTile.transform;
                }

                foreach (KeyValuePair<Vector2, FurnitureInfo> _dicionary in _info.FurnitureInfoDic)
                {
                    Vector2 _pos = _dicionary.Key;

                    string _name = _dicionary.Value.name;

                    int _rotate = (int)_dicionary.Value.dir * 90;

                    obfurniture = Resources.Load("MapTools/Furniture/" + _name) as GameObject;

                    GameObject _furniture = Instantiate(obfurniture, new Vector2(_pos.x - 0.5f, _pos.y - 0.5f), Quaternion.Euler(new Vector3(-90 + _rotate, 90, -90)));
                    _furniture.transform.parent = mapBoardArr[x + 25, y + 25].obTile.transform;
                    _furniture.name = string.Format("{0} // {1} {2}", _pos.x, _pos.y, _name);
                }
            }
        }

        isEditData = true;
        isLoadingData = false;
    }
}
