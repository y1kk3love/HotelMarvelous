using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    private Tile[,] TileArr;
    public List<Tile> PathList = new List<Tile>();
    private List<Tile> OpenList = new List<Tile>();
    private List<Tile> CloseList = new List<Tile>();

    int sx, sy, xlen, ylen;

    public void ResetGridMap(List<TileInfo> _list)
    {
        if (_list != null && _list[0].roomType != ROOMTYPE.NPC)
        {
            sx = 0;
            sy = 0;
            int bx = 0;
            int by = 0;

            for (int i = 0; i < _list.Count; i++)
            {
                int x = (int)_list[i].position.x;
                int y = (int)_list[i].position.y;

                if (x < sx) { sx = x; }
                if (x > bx) { bx = x; }
                if (y < sy) { sy = y; }
                if (y > by) { by = y; }
            }

            xlen = Mathf.Abs(sx - bx) + 18;
            ylen = Mathf.Abs(sy - by) + 18;

            TileArr = new Tile[xlen, ylen];

            for (int x = 0; x < xlen; x++)
            {
                for (int y = 0; y < ylen; y++)
                {
                    TileArr[x, y] = new Tile(true, x, y);
                }
            }

            SetWallInMap(_list);
        }
        else
        {
            GameObject manager = GameObject.Find("DungeonManager");

            if(manager != null)
            { 
                List<TileInfo> list = manager.GetComponent<DungeonMaker>().GetTileMap();

                ResetGridMap(list);
            }
        }
    }

    private void SetWallInMap(List<TileInfo> _list)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            int tx = (int)_list[i].position.x;
            int ty = (int)_list[i].position.y;

            int curArrX = tx - sx;
            int curArrY = ty - sy;

            for (int x = curArrX; x < curArrX + 18; x++)
            {
                for (int y = curArrY; y < curArrY + 18; y++)
                {
                    TileArr[x, y].isWall = false;

                    //GameObject a = Instantiate(cube, new Vector3(x - 8.5f, 0, y - 8.5f), Quaternion.identity);
                    //격자 위에서의 실제 위치는  x + 8.5f
                    //(받아온 좌표)올림 - 0.5
                }
            }
        }
    }

    #region [PathFind]

    public List<Tile> PathFind(Tile _StartTile, Tile _TargetTile)
    {
        if(TileArr == null)
        {
            ResetGridMap(null);
        }

        if (_StartTile == null || _TargetTile == null)
        {
            return null;
        }

        PathList.Clear();
        OpenList.Clear();
        CloseList.Clear();

        Tile _CurTile = _StartTile;
        _CurTile.DistanceCalculator(_CurTile, _TargetTile);
        OpenList.Add(_CurTile);

        while (OpenList.Count > 0)
        {
            _CurTile = OpenList[0];

            for (int i = 1; i < OpenList.Count; i++)
            {
                if (OpenList[i].totalDis <= _CurTile.totalDis && OpenList[i].remainDis < _CurTile.remainDis)
                {
                    _CurTile = OpenList[i];
                }
            }

            OpenList.Remove(_CurTile);
            CloseList.Add(_CurTile);

            if (_CurTile.X == _TargetTile.X && _CurTile.Y == _TargetTile.Y)
            {
                Tile curTargetTile = _CurTile;

                while (curTargetTile != _StartTile)
                {
                    PathList.Add(curTargetTile);
                    curTargetTile = curTargetTile.ParentTile;
                }

                PathList.Add(_StartTile);
                PathList.Reverse();

                Debug.Log(PathList.Count);
                return PathList;
            }

            OpenListAdd(_CurTile.X + 1, _CurTile.Y + 1, _CurTile, _TargetTile, true);
            OpenListAdd(_CurTile.X + 1, _CurTile.Y + 1, _CurTile, _TargetTile, true);
            OpenListAdd(_CurTile.X - 1, _CurTile.Y - 1, _CurTile, _TargetTile, true);
            OpenListAdd(_CurTile.X + 1, _CurTile.Y - 1, _CurTile, _TargetTile, true);

            OpenListAdd(_CurTile.X, _CurTile.Y + 1, _CurTile, _TargetTile, false);
            OpenListAdd(_CurTile.X + 1, _CurTile.Y, _CurTile, _TargetTile, false);
            OpenListAdd(_CurTile.X, _CurTile.Y - 1, _CurTile, _TargetTile, false);
            OpenListAdd(_CurTile.X - 1, _CurTile.Y, _CurTile, _TargetTile, false);
        }

        return null;
    }

    private void OpenListAdd(int _x, int _y, Tile _CurTile, Tile _TargetTile, bool _cross)
    {
        if (_x >= 0 && _x < xlen - 1 && _y >= 0 && _y < ylen - 1 && !TileArr[_x, _y].isWall && !CloseList.Contains(TileArr[_x, _y]))
        {
            Tile neighborTile = TileArr[_x, _y];

            int curCost = 0;

            if (_cross) { curCost = _CurTile.curDis + 14; }
            else { curCost = _CurTile.curDis + 10; }

            if (curCost < neighborTile.curDis || !OpenList.Contains(neighborTile))
            {
                neighborTile.curDis = curCost;

                neighborTile.DistanceCalculator(neighborTile, _TargetTile);

                neighborTile.ParentTile = _CurTile;

                OpenList.Add(neighborTile);
            }
        }
    }

    #endregion
}

public class Tile
{
    public int X, Y = 0;

    public bool isWall = false;

    public int curDis, remainDis = 0;
    public int totalDis { get { return curDis + remainDis; } }

    public Tile ParentTile;

    public Tile(bool _iswall, int _x, int _y)
    {
        isWall = _iswall;
        X = _x;
        Y = _y;
    }

    public void DistanceCalculator(Tile start, Tile target)
    {
        remainDis = (Mathf.Abs(start.X - target.Y) + Mathf.Abs(start.Y - target.Y)) * 10;
    }
}