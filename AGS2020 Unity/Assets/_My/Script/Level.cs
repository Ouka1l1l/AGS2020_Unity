using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class Level : MonoBehaviour
{
    /// <summary>
    /// 地形情報
    /// </summary>
    public enum TerrainType
    {
        Wall,   //壁
        Floor,  //床
        Road,   //道
        Event,  //イベント
        Item,   //アイテム
        Max
    }

    /// <summary>
    /// 地形データ
    /// </summary>
    private TerrainType[,] _terrainData;

    private GameObject[,] _miniMapMask;

    /// <summary>
    /// イベントデータ
    /// </summary>
    private Event[,] _eventData;

    /// <summary>
    /// キャラクタデータ
    /// </summary>
    private int[,] _characterData;

    /// <summary>
    /// アイテムデータ
    /// </summary>
    private Item[,] _itemData;

    /// <summary>
    /// 区画
    /// </summary>
    public List<Section> _sections { get; private set; }

    /// <summary>
    /// 部屋と部屋の余白
    /// </summary>
    private Vector2Int Margin = new Vector2Int(6,6);
    /// <summary>
    /// 部屋の最小サイズ
    /// </summary>
    private Vector2Int RoomMin = new Vector2Int(4,4);
    /// <summary>
    /// 部屋の最大サイズ
    /// </summary>
    private Vector2Int RoomMax = new Vector2Int(10, 10);

    public List<Enemy> _enemies { get; private set; }

    /// <summary>
    /// 敵の数 最小値
    /// </summary>
    private int _enemyMin = 5;
    /// <summary>
    /// 敵の数 最大値
    /// </summary>
    private int _enemyMax = 8;

    /// <summary>
    /// イベント数 最小値
    /// </summary>
    private int _eventMin = 10;
    /// <summary>
    /// イベント数 最大値
    /// </summary>
    private int _eventMax = 20;

    public Material material;

    public Vector2Int staisPos;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        foreach(Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
    }

    /// <summary>
    /// 現在の部屋の区画番号を取得
    /// </summary>
    /// <param name="x"></param> X座標
    /// <param name="y"></param> Y座標
    /// <returns></returns> 現在の部屋の区画番号 部屋の中ではない場合は-1
    public int GetRoomNo(int x,int y)
    {
        Vector2Int grid = DungeonManager.instance.GetGrid(x, y);
        var section = _sections.Find(s => s._roomData.left <= grid.x && s._roomData.right >= grid.x 
                                && s._roomData.top <= grid.y && s._roomData.bottom >= grid.y);
        if(section != null)
        {
            return section._no;
        }

        return -1;
    }
    public int GetRoomNo(float x, float y)
    {
        return GetRoomNo((int)x, (int)y);
    }

    /// <summary>
    /// T型のデータを取得
    /// </summary>
    /// <typeparam name="T"></typeparam> 取得したいデータの型
    /// <param name="x"></param> 横の座標
    /// <param name="y"></param> 縦の座標
    /// <param name="dataList"></param> 取得したいデータの配列
    /// <returns></returns> T型のデータ
    private T GetData<T>(int x,int y,T[,] dataList)
    {
        var grid = DungeonManager.instance.GetGrid(x, y);
        return dataList[grid.y, grid.x];
    }

    /// <summary>
    /// 地形情報を取得
    /// </summary>
    /// <param name="x"></param> 横の座標
    /// <param name="z"></param> 縦の座標
    /// <returns></returns> 地形情報
    public TerrainType GetTerrainData(int x,int y)
    {
        return GetData(x, y, _terrainData);
    }
    public TerrainType GetTerrainData(Vector2Int vec)
    {
        return GetData(vec.x, vec.y, _terrainData);
    }
    public TerrainType GetTerrainData(float x, float y)
    {
        return GetData((int)y, (int)x, _terrainData);
    }

    /// <summary>
    /// キャラクタデータを取得
    /// </summary>
    /// <param name="x"></param> 横の座標
    /// <param name="z"></param> 縦の座標
    /// <returns></returns> キャラクタデータ
    public int GetCharacterData(int x, int y)
    {
        return GetData(x, y, _characterData);
    }
    public int GetCharacterData(Vector2Int vec)
    {
        return GetData(vec.x, vec.y, _characterData);
    }
    public int GetCharacterData(float x, float y)
    {
        return GetData((int)y, (int)x, _characterData);
    }

    /// <summary>
    /// アイテムデータを取得
    /// </summary>
    /// <param name="x"></param> 横の座標
    /// <param name="z"></param> 縦の座標
    /// <returns></returns> アイテムデータ
    public Item GetItemData(int x, int y)
    {
        return GetData(x, y, _itemData);
    }
    public Item GetItemData(Vector2Int pos)
    {
        return GetData(pos.x, pos.y, _itemData);
    }

    /// <summary>
    /// 周辺のデータを取得
    /// </summary>
    /// <typeparam name="T"></typeparam> 取得したいデータの型
    /// <param name="x"></param> 中心X座標
    /// <param name="y"></param> 中心Y座標
    /// <param name="dataList"></param> 取得したいデータの配列
    /// <param name="rangeX"></param> 取得したい横範囲
    /// <param name="rangeY"></param> 取得したい縦範囲
    /// <returns></returns> 周辺のデータリスト
    private Dictionary<Vector2Int,T> GetSurroundingData<T>(int x,int y,T[,] dataList,int rangeX,int rangeY)
    {
        Dictionary<Vector2Int, T> ret = new Dictionary<Vector2Int, T>();
        for (int ry = -rangeY; ry <= rangeY; ry++)
        {
            for (int rx = -rangeX; rx <= rangeX; rx++)
            {
                Vector2Int key =  new Vector2Int(rx, ry);
                ret.Add(key, GetData(x + rx, y + ry, dataList));
            }
        }

        return ret;
    }

    /// <summary>
    /// 周辺の地形データを取得
    /// </summary>
    /// <param name="x"></param> 中心X座標
    /// <param name="y"></param> 中心Y座標
    /// <param name="rangeX"></param> 取得したい横範囲
    /// <param name="rangeY"></param> 取得したい縦範囲
    /// <returns></returns> 周辺の地形データリスト
    public Dictionary<Vector2Int, TerrainType> GetSurroundingTerrainData(int x, int y, int rangeX, int rangeY)
    {
        return GetSurroundingData(x, y, _terrainData, rangeX, rangeY);
    }

    /// <summary>
    /// 周辺のキャラデータを取得
    /// </summary>
    /// <param name="x"></param> 中心X座標
    /// <param name="y"></param> 中心Y座標
    /// <param name="rangeX"></param> 取得したい横範囲
    /// <param name="rangeY"></param> 取得したい縦範囲
    /// <returns></returns> 周辺のキャラデータリスト
    public Dictionary<Vector2Int, int> GetSurroundingCharacterData(int x,int y, int rangeX, int rangeY)
    {
        return GetSurroundingData(x, y, _characterData, rangeX, rangeY);
    }
    public Dictionary<Vector2Int, int> GetSurroundingCharacterData(float x, float y, int rangeX, int rangeY)
    {
        return GetSurroundingData((int)x, (int)y, _characterData, rangeX, rangeY);
    }

    /// <summary>
    /// T型の配列の指定の場所のデータを変更
    /// </summary>
    /// <typeparam name="T"></typeparam> 変更したいデータの型
    /// <param name="x"></param> 横の座標
    /// <param name="y"></param> 縦の座標
    /// <param name="dataList"></param> 変更したいデータ配列
    /// <param name="data"></param> 変更値
    private void SetData<T>(int x, int y, T[,] dataList, T data)
    {
        var grid = DungeonManager.instance.GetGrid(x, y);
        dataList[grid.y, grid.x] = data;
    }

    /// <summary>
    /// 地形データをセット
    /// </summary>
    /// <param name="x"></param> 横の座標
    /// <param name="z"></param> 縦の座標
    /// <param name="data"></param> セットする地形データ
    public void SetTerrainData(int x,int y,TerrainType data)
    {
        SetData(x, y, _terrainData, data);
    }
    public void SetTerrainData(Vector2Int pos, TerrainType data)
    {
        SetData(pos.x, pos.y, _terrainData, data);
    }

    /// <summary>
    /// キャラクタデータをセット
    /// </summary>
    /// <param name="x"></param> 横の座標
    /// <param name="z"></param> 縦の座標
    /// <param name="data"></param> セットする値
    public void SetCharacterData(int x, int y, int data)
    {
        SetData(x, y, _characterData, data);
    }
    public void SetCharacterData(float x, float y, int data)
    {
        SetData((int)x, (int)y, _characterData, data);
    }

    /// <summary>
    /// アイテムデータをセット
    /// </summary>
    /// <param name="x"></param> 横の座標
    /// <param name="y"></param> 縦の座標
    /// <param name="data"></param> セットするアイテム
    public void SetItemData(int x, int y,Item data)
    {
        SetData(x, y, _itemData, data);
    }
    public void SetItemData(Vector2Int pos, Item data)
    {
        SetData(pos.x, pos.y, _itemData, data);
    }

    /// <summary>
    /// 区画を分割する
    /// </summary>
    /// <param name="divisionNum"></param> 分割回数
    private void SectionDivision(int divisionNum)
    {
        _sections.Add(new Section(0, new Section.Rect(0, _terrainData.GetLength(1) - 1, _terrainData.GetLength(0) - 1, 0)));

        for (int d = 1; d < divisionNum; d++)
        {
            var index = GetMaxWidthIndex() ;

            if (SelectDivisionDir(index))
            {
                if(!SectionDivisionX(_sections[index], d))
                {
                    break;
                }
            }
            else
            {
                if(!SectionDivisionY(_sections[index], d))
                {
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 面積が一番大きい区画を取得
    /// </summary>
    /// <returns></returns> 面積が一番大きい区画の番号
    private int GetMaxWidthIndex()
    {
        int index = 0;
        int max = _sections[0]._sectionData.Area;
        for (int s = 1; s < _sections.Count; s++)
        {
            if (max < _sections[s]._sectionData.Area)
            {
                index = s;
                max = _sections[s]._sectionData.Area;
            }
        }
        return index;
    }

    /// <summary>
    /// 分割方向を決定
    /// </summary>
    /// <param name="index"></param> 分割する区画番号
    /// <returns></returns> true 横 false 縦
    private bool SelectDivisionDir(int index)
    {
        return _sections[index]._sectionData.width > _sections[index]._sectionData.height;
    }

    /// <summary>
    /// 横方向に区画を分割する
    /// </summary>
    /// <param name="section"></param> 分割する区画
    /// <param name="no"></param> 新しい区画の番号
    /// <returns></returns> 分割できたか
    private bool SectionDivisionX(Section section, int no)
    {
        //区画が分割できるサイズがあるか
        if(section._sectionData.width <= (RoomMin.x + Margin.x) * 2)
        {
            return false;
        }

        int min = (RoomMin.y + Margin.y) - 1;

        var rect = section._sectionData;
        int divPointMax = rect.right - min;
        int divPointMin = rect.left + min;

        //部屋の最大サイズを超えないようにする
        int width = divPointMax - divPointMin;
        width = Mathf.Min(width, (RoomMax.x + Margin.x) - 1);

        int divPoint = divPointMin + Random.Range(0, width + 1);
        Section newSection = new Section(no, new Section.Rect(rect.top, rect.right, rect.bottom, divPoint));
        _sections.Add(newSection);
        newSection.SetAdjacentSection(Dir.Left, section._no);
        section._sectionData.right = divPoint;
        int oldNo = section.SetAdjacentSection(Dir.Right, no);

        if (oldNo != -1)
        {
            newSection.SetAdjacentSection(Dir.Right, oldNo);
            _sections[oldNo].SetAdjacentSection(Dir.Left, no);
        }


        return true;
    }

    /// <summary>
    /// 縦方向に区画を分割する
    /// </summary>
    /// <param name="section"></param> 分割する区画
    /// <param name="no"></param> 新しい区画の番号
    /// <returns></returns> 分割できたか
    private bool SectionDivisionY(Section section, int no)
    {
        //区画が分割できるサイズがあるか
        if (section._sectionData.height <= ((RoomMin.y + Margin.y) * 2))
        {
            return false;
        }

        int min = (RoomMin.y + Margin.y) - 1;

        var rect = section._sectionData;
        int divPointMin = rect.top + min;
        int divPointMax = rect.bottom - min;

        //部屋の最大サイズを超えないようにする
        int width = divPointMax - divPointMin;
        width = Mathf.Min(width, (RoomMax.x + Margin.x) - 1);

        int divPoint = divPointMin + Random.Range(0, width + 1);

        Section newSection = new Section(no,new Section.Rect(divPoint, rect.right, rect.bottom, rect.left));
        _sections.Add(newSection);
        newSection.SetAdjacentSection(Dir.Top, section._no);
        section._sectionData.bottom = divPoint;
        int oldNo = section.SetAdjacentSection(Dir.Bottom, no);

        if (oldNo != -1)
        {
            newSection.SetAdjacentSection(Dir.Bottom, oldNo);
            _sections[oldNo].SetAdjacentSection(Dir.Top, no);

        }

        return true;
    }

    /// <summary>
    /// 道を繋げる
    /// </summary>
    /// <param name="room1RandomMin"></param> 一つ目の道の開始地点のランダム座標の最小値
    /// <param name="room1RandomMax"></param> 一つ目の道の開始地点のランダム座標の最大値
    /// <param name="room1RoadStart"></param> 一つ目の道の開始地点の固定座標の値
    /// <param name="room2RandomMin"></param> 二つ目の道の開始地点のランダム座標の最小値
    /// <param name="room2RandomMax"></param> 二つ目の道の開始地点のランダム座標の最大値
    /// <param name="room2RoadStart"></param> 二つ目の道の開始地点の固定座標の値
    /// <param name="adjacentPoint"></param> 道同士を繋げるときの基準
    /// <param name="isRandomX"></param> X座標をランダムにするか
    private void ConnectingRoad(int room1RandomMin,int room1RandomMax,int room1RoadStart,int room2RandomMin,int room2RandomMax,int room2RoadStart,int adjacentPoint,bool isRandomX)
    {
        //道の開始地点のランダム座標の値を決定
        Func<int, int, int, int> RandomRoadStartPos = (int randomMin, int randomMax, int start) =>
          {
              TerrainType startTerrainType;
              int ret;

              //開始地点は何もない床
              do
              {
                  ret = Random.Range(randomMin, (randomMax + 1));
                  if (isRandomX)
                  {
                      startTerrainType = _terrainData[start, ret];
                  }
                  else
                  {
                      startTerrainType = _terrainData[ret, start];
                  }

              } while (startTerrainType != TerrainType.Floor);

              return ret;
          };

        int tmp1 = RandomRoadStartPos(room1RandomMin, room1RandomMax, room1RoadStart);
        for (int r = room1RoadStart; r >= adjacentPoint; r--)
        {
            if (isRandomX)
            {
                if(_terrainData[r, tmp1] == TerrainType.Wall)
                {
                    _terrainData[r, tmp1] = TerrainType.Road;
                }
            }
            else
            {
                if (_terrainData[tmp1, r] == TerrainType.Wall)
                {
                    _terrainData[tmp1, r] = TerrainType.Road;
                }
            }
        }

        int tmp2 = RandomRoadStartPos(room2RandomMin, room2RandomMax, room2RoadStart);
        for (int r = room2RoadStart; r <= adjacentPoint; r++)
        {
            if (isRandomX)
            {
                if (_terrainData[r, tmp2] == TerrainType.Wall)
                {
                    _terrainData[r, tmp2] = TerrainType.Road;
                }
            }
            else
            {
                if (_terrainData[tmp2, r] == TerrainType.Wall)
                {
                    _terrainData[tmp2, r] = TerrainType.Road;
                }
            }
        }

        int adjacentStart;
        int adjacentEnd;
        if (tmp1 < tmp2)
        {
            adjacentStart = tmp1;
            adjacentEnd = tmp2;
        }
        else
        {
            adjacentStart = tmp2;
            adjacentEnd = tmp1;
        }

        if(isRandomX)
        {
            for (int t = adjacentStart; t <= adjacentEnd; t++)
            {
                if (_terrainData[adjacentPoint, t] == TerrainType.Wall)
                {
                    _terrainData[adjacentPoint, t] = TerrainType.Road;
                }
            }
        }
        else
        {
            for (int t = adjacentStart; t <= adjacentEnd; t++)
            {
                if (_terrainData[t, adjacentPoint] == TerrainType.Wall)
                {
                    _terrainData[t, adjacentPoint] = TerrainType.Road;
                }
            }
        }
    }

    /// <summary>
    /// 道を作成
    /// </summary>
    private void CreateRoad()
    {
        for (int index = 0; index < _sections.Count; index++)
        {
            //区画の情報
            var sectionData = _sections[index]._sectionData;

            //部屋の情報
            var room1 = _sections[index]._roomData;

            //隣接部屋の情報
            Section.Rect room2;

            if (_sections[index]._adjacentSections.ContainsKey(Dir.Top))
            {
                room2 = _sections[_sections[index]._adjacentSections[Dir.Top]]._roomData;
                ConnectingRoad(room1.left, room1.right, room1.top, room2.left, room2.right, room2.bottom, sectionData.top, true);
            }
            if (_sections[index]._adjacentSections.ContainsKey(Dir.Left))
            {
                room2 = _sections[_sections[index]._adjacentSections[Dir.Left]]._roomData;
                ConnectingRoad(room1.top, room1.bottom, room1.left, room2.top, room2.bottom, room2.right, sectionData.left, false);
            }
        }
    }

    /// <summary>
    /// 地形データを作成
    /// </summary>
    /// <param name="mapSize"></param> マップの大きさ
    /// <param name="divisionNum"></param> 部屋の数
    public void CreateTerrainData(Vector2Int mapSize,int divisionNum)
    {
        _terrainData = new TerrainType[mapSize.y, mapSize.x];
        _characterData = new int[mapSize.y, mapSize.x];
        _sections = new List<Section>();

        SectionDivision(divisionNum);
        foreach (var section in _sections)
        {
            section.CreateRoom(RoomMin, RoomMax, Margin);

            for (int y = section._roomData.top; y <= section._roomData.bottom; y++)
            {
                for (int x = section._roomData.left; x <= section._roomData.right; x++)
                {
                    _terrainData[y, x] = TerrainType.Floor;
                }
            }
        }

        CreateEvent(mapSize);

        CreateItem(mapSize);

        CreateRoad();
    }

    /// <summary>
    /// ランダムな床の座標を取得
    /// </summary>
    /// <returns></returns>
    public Vector2Int GetRandomFloorPos()
    {
        int x;
        int y;
        do
        {
            int no = Random.Range(0, _sections.Count);
            var room = _sections[no]._roomData;

            x = Random.Range(room.left, room.right + 1);
            y = Random.Range(room.top, room.bottom + 1);

        } while (_terrainData[y, x] != TerrainType.Floor);

        return new Vector2Int(x, -y);
    }

    /// <summary>
    /// イベントを作成
    /// </summary>
    /// <param name="mapSize"></param> マップのサイズ
    private void CreateEvent(Vector2Int mapSize)
    {
        if (_eventData != null)
        {
            foreach (var e in _eventData)
            {
                if (e != null)
                {
                    Destroy(e.gameObject);
                }
            }
        }
        _eventData = new Event[mapSize.y, mapSize.x];

        CreateStairs();

        int eventNum = Random.Range(_eventMin, _eventMax + 1);
        for (int e = 0; e < eventNum; e++)
        {
            var pos = GetRandomFloorPos();
            var grid = DungeonManager.instance.GetGrid(pos);
            var needleFloor = Instantiate((GameObject)Resources.Load("NeedleFloor")).GetComponent<NeedleFloor>();
            needleFloor.SetPos(pos.x, pos.y);
            _terrainData[grid.y, grid.x] = TerrainType.Event;
            _eventData[grid.y, grid.x] = needleFloor;
        }
    }

    /// <summary>
    /// 階段を作成
    /// </summary>
    private void CreateStairs()
    {

        int no = Random.Range(0, _sections.Count);
        var room = _sections[no]._roomData;

        Vector2Int grid = new Vector2Int();
        //部屋の端は含めないことにする
        grid.x = Random.Range(room.left + 1, room.right);
        grid.y = Random.Range(room.top + 1, room.bottom);

        var pos = new Vector2Int(grid.x, -grid.y);
        _terrainData[grid.y, grid.x] = TerrainType.Event;
        Stairs stairs = Instantiate((GameObject)Resources.Load("Stairs")).GetComponent<Stairs>();
        stairs.SetPos(pos.x, pos.y);
        _eventData[grid.y, grid.x] = stairs;

        staisPos = pos;
    }

    /// <summary>
    /// アイテムを生成
    /// </summary>
    private void CreateItem(Vector2Int mapSize)
    {
        if (_itemData != null)
        {
            foreach (var item in _itemData)
            {
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }
        }
        _itemData = new Item[mapSize.y, mapSize.x];

        var pos = GetRandomFloorPos();
        pos = new Vector2Int(staisPos.x, staisPos.y - 1);
        var grid = DungeonManager.instance.GetGrid(pos);

        _terrainData[grid.y, grid.x] = TerrainType.Item;
        var Portion = Instantiate((GameObject)Resources.Load("Portion")).GetComponent<Portion>();
        Portion.SetPos(pos.x, pos.y);
        _itemData[grid.y, grid.x] = Portion;
    }

    /// <summary>
    /// 階層を作成
    /// </summary>
    /// <param name="mapSize"></param> マップサイズ
    /// <param name="divisionNum"></param> 部屋の数
    public void CreateLevel(Vector2Int mapSize, int divisionNum)
    {
        UIManager.instance.SetMiniMapCamera(mapSize);

        foreach (Transform child in transform)
        {
            foreach (Transform Grandchild in child)
            {
                Destroy(Grandchild.gameObject);
            }
            Destroy(child.gameObject);
        }

        CreateTerrainData(mapSize, divisionNum);

        _miniMapMask = new GameObject[mapSize.y, mapSize.x];

        //地形情報どうりにブロックを設置
        for (int y = 0; y < mapSize.y; y++)
        {
            for (int x = 0; x < mapSize.x; x++)
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(x, -1, -y);
                cube.transform.SetParent(transform);
                cube.GetComponent<Renderer>().material = material;

                _miniMapMask[y, x] = cube;

                if (_terrainData[y, x] == TerrainType.Wall)
                {
                    var Wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Wall.transform.position = new Vector3(x, 0, -y);
                    Wall.transform.SetParent(cube.transform);
                }

                //キャラクタデータの初期化
                _characterData[y, x] = -1;
            }
        }

        //敵を配置
        EnemysSpawn();
    }

    /// <summary>
    /// 敵を増員
    /// </summary>
    public void EnemyIncrease()
    {
        for (int i = 0; i < _enemies.Count; i++)
        {
            if (_enemies[i] == null)
            {
                Enemy enemy = Instantiate((GameObject)Resources.Load("TestEnemy")).GetComponent<Enemy>();
                do
                {
                    enemy.Spawn(0, i);
                } while (enemy._roomNo == DungeonManager.instance._player._roomNo);

                _enemies[i] = enemy;

                break;
            }
        }
    }

    /// <summary>
    /// 敵を配置
    /// </summary>
    private void EnemysSpawn()
    {
        if (_enemies != null)
        {
            foreach (var enemy in _enemies)
            {
                if (enemy != null)
                {
                    Destroy(enemy.gameObject);
                }
            }
        }

        _enemies = new List<Enemy>();

        int enemyCount = Random.Range(_enemyMin, _enemyMax + 1);
        for (int e = 1; e <= enemyCount; e++)
        {
            Enemy enemy = Instantiate((GameObject)Resources.Load("TestEnemy")).GetComponent<Enemy>();
            enemy.Spawn(0, e);
            _enemies.Add(enemy);
        }
    }

    /// <summary>
    /// 該当マスのイベントを実行する
    /// </summary>
    /// <param name="x"></param> 横の座標
    /// <param name="y"></param> 縦の座標
    /// <param name="character"></param> 該当マスを踏んだキャラ
    public void EventExecution(int x, int y,Character character)
    {
        var grid = DungeonManager.instance.GetGrid(x, y);
        _eventData[grid.y, grid.x].Execution(character);
    }

    private void UpdateMiniMap(int startX,int startY,int endX,int endY)
    {
        int maskLayer = LayerMask.NameToLayer("MiniMapMask");

        for (int my = startY; my <= endY; my++)
        {
            for (int mx = startX; mx <= endX; mx++)
            {
                if (_terrainData[my, mx] != TerrainType.Wall)
                {
                    _miniMapMask[my, mx].layer = maskLayer;
                }
            }
        }
    }

    public void UpdateMiniMap(int x,int y)
    {
        var grid = DungeonManager.instance.GetGrid(x, y);

        int maskLayer = LayerMask.NameToLayer("MiniMapMask");

        UpdateMiniMap((grid.x - 1), (grid.y - 1), (grid.x + 1), (grid.y + 1));
    }

    public void UpdateMiniMap(int roomNo)
    {
        var room = _sections[roomNo]._roomData;

        int maskLayer = LayerMask.NameToLayer("MiniMapMask");

        if(_miniMapMask[room.height / 2, room.width / 2].layer == maskLayer)
        {
            return;
        }

        UpdateMiniMap(room.left - 1, room.top - 1, room.right + 1, room.bottom + 1);
    }
}
