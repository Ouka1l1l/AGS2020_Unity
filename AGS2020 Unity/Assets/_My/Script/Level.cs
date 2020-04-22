using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Level : MonoBehaviour
{
    /// <summary>
    /// 地形情報
    /// </summary>
    public enum TerrainType
    {
        Wall,
        Floor,
        Event,
        Max
    }

    /// <summary>
    /// 地形データ
    /// </summary>
    public TerrainType[,] _terrainData { get; private set; }

    /// <summary>
    /// イベントデータ
    /// </summary>
    public Event[,] _eventData { get; private set; }

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

    public Material material;

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
    /// 地形情報を取得
    /// </summary>
    /// <param name="x"></param> 横のマス目
    /// <param name="z"></param> 縦のマス目
    /// <returns></returns> 地形情報
    public TerrainType GetTerrainData(int x,int z)
    {
        return _terrainData[z, x];
    }

    public TerrainType GetTerrainData(Vector2Int vec)
    {
        return GetTerrainData(vec.x, vec.y);
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
        section.SetAdjacentSection(Dir.Right, no);

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
        section.SetAdjacentSection(Dir.Bottom, no);

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
        int tmp1 = Random.Range(room1RandomMin, (room1RandomMax + 1));
        for (int r = room1RoadStart; r >= adjacentPoint; r--)
        {
            if (isRandomX)
            {
                _terrainData[r, tmp1] = TerrainType.Floor;
            }
            else
            {
                _terrainData[tmp1, r] = TerrainType.Floor;
            }
        }

        int tmp2 = Random.Range(room2RandomMin, (room2RandomMax + 1));
        for (int r = room2RoadStart; r <= adjacentPoint; r++)
        {
            if (isRandomX)
            {
                _terrainData[r, tmp2] = TerrainType.Floor;
            }
            else
            {
                _terrainData[tmp2, r] = TerrainType.Floor;
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
                _terrainData[adjacentPoint, t] = TerrainType.Floor;
            }
        }
        else
        {
            for (int t = adjacentStart; t <= adjacentEnd; t++)
            {
                _terrainData[t, adjacentPoint] = TerrainType.Floor;
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
                ConnectingRoad(room1.top, room1.bottom, room1.right, room2.top, room2.bottom, room2.left, sectionData.left, false);
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
        _eventData = new Event[mapSize.y, mapSize.x];
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

        CreateStairs();

        CreateRoad();
    }

    private void CreateStairs()
    {
        int no = Random.Range(0, _sections.Count);
        var room = _sections[no]._roomData;

        //部屋の端は含めないことにする
        int x = Random.Range(room.left + 1, room.right);
        int y = Random.Range(room.top + 1, room.bottom);
        _terrainData[y, x] = TerrainType.Event;
        _eventData[y, x] = new Stairs();
    }

    public void CreateLevel(Vector2Int mapSize, int divisionNum)
    {
        foreach (Transform child in transform)
        {
            foreach (Transform Grandchild in child)
            {
                Destroy(Grandchild.gameObject);
            }
            Destroy(child.gameObject);
        }

        CreateTerrainData(mapSize, divisionNum);

        //地形情報どうりにブロックを設置
        for (int y = 0; y < _terrainData.GetLength(0); y++)
        {
            for (int x = 0; x < _terrainData.GetLength(1); x++)
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(x, -1, -y);
                cube.transform.SetParent(transform);
                cube.GetComponent<Renderer>().material = material;

                if (_terrainData[y, x] == TerrainType.Wall)
                {
                    var Wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Wall.transform.position = new Vector3(x, 0, -y);
                    Wall.transform.SetParent(cube.transform);
                }
                else if (_terrainData[y, x] == TerrainType.Event)
                {
                    var Sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    Sphere.transform.position = new Vector3(x, 1, -y);
                    Sphere.transform.SetParent(cube.transform);
                }
            }
        }
    }
}
