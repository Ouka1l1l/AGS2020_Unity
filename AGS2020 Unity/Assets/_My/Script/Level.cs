using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    /// <summary>
    /// 地形情報
    /// </summary>
    public enum TerrainData
    {
        floor,
        Wall,
        Max
    }

    /// <summary>
    /// 地形データ
    /// </summary>
    public TerrainData[,] _level { get; private set; }

    /// <summary>
    /// 区画
    /// </summary>
    private List<Section> _sections;

    /// <summary>
    /// 部屋と部屋の余白
    /// </summary>
    private Vector2Int Margin = new Vector2Int(3,3);
    /// <summary>
    /// 部屋の最小サイズ
    /// </summary>
    private Vector2Int RoomMin = new Vector2Int(4,4);
    /// <summary>
    /// 部屋の最大サイズ
    /// </summary>
    private Vector2Int RoomMax = new Vector2Int(8, 8);

    public Material material;

    private void Awake()
    {
        _level = new TerrainData[25, 50];
        _sections = new List<Section>();

        for (int z = 0; z < _level.GetLength(0); z++)
        {
            _level[z, 0] = TerrainData.Wall;
            _level[z, _level.GetLength(1) - 1] = TerrainData.Wall;
        }

        for (int x = 0; x < _level.GetLength(1); x++)
        {
            _level[0, x] = TerrainData.Wall;
            _level[_level.GetLength(0) - 1, x] = TerrainData.Wall;
        }

        for (int z = 0; z < _level.GetLength(0); z++)
        {
            for (int x = 0; x < _level.GetLength(1); x++)
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(x, -1, -z);
                cube.transform.SetParent(transform);
                cube.GetComponent<Renderer>().material = material;

                //if (_level[z, x] == TerrainData.Wall)
                //{
                //    var Wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //    Wall.transform.position = new Vector3(x, 1, -z);
                //    Wall.transform.SetParent(cube.transform);
                //}
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _sections.Add(new Section(0, new Section.Rect(0, _level.GetLength(1), _level.GetLength(0), 0)));
        SectionDivision(5);

        for (int s = 0; s < _sections.Count; s++)
        {
            if (s % 2 == 0)
            {
                var section = _sections[s]._section;
                for (int z = section.top; z < section.bottom; z++)
                {
                    for (int x = section.left; x < section.right; x++)
                    {
                        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        cube.transform.position = new Vector3(x, 0, -z);
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// 地形情報を取得
    /// </summary>
    /// <param name="x"></param> 横のマス目
    /// <param name="z"></param> 縦のマス目
    /// <returns></returns> 地形情報
    public TerrainData GetTerrainData(int x,int z)
    {
        return _level[z, x];
    }

    /// <summary>
    /// 区画を分割する
    /// </summary>
    /// <param name="divisionNum"></param> 分割回数
    private void SectionDivision(int divisionNum)
    {
        for (int d = 0; d < divisionNum; d++)
        {
            var index = GetMaxSectionIndex();

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
    private int GetMaxSectionIndex()
    {
        int index = 0;
        int max = _sections[0]._room.Area;
        for (int s = 1; s < _sections.Count; s++)
        {
            if (max > _sections[s]._room.Area)
            {
                index = s;
                max = _sections[s]._room.Area;
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
        return _sections[index]._room.width > _sections[index]._room.right;
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
        if(section._section.width < (RoomMin.x + Margin.x))
        {
            return false;
        }

        var rect = section._section;
        int divPointRight = rect.right - (RoomMin.x + Margin.x);
        int divPointLeft = rect.left + (RoomMin.x + Margin.x);

        //部屋の最大サイズを超えないようにする
        int width = divPointRight - divPointLeft;
        width = Mathf.Min(width, RoomMax.x + Margin.x);

        int divPoint = divPointLeft + Random.Range(0, width + 1);
        _sections.Add(new Section(no, new Section.Rect(rect.top, rect.right, rect.bottom, divPoint)));
        section._section.right = divPoint;
        section._adjacentSection.Add(no);

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
        if (section._section.height < (RoomMin.y + Margin.y))
        {
            return false;
        }

        var rect = section._section;
        int divPointTop = rect.top + (RoomMin.y + Margin.y);
        int divPointBottom = rect.bottom - (RoomMin.y + Margin.y);

        //部屋の最大サイズを超えないようにする
        int width = divPointBottom - divPointTop;
        width = Mathf.Min(width, RoomMax.x + Margin.x);

        int divPoint = divPointTop + Random.Range(0, width + 1);

        Section newSection = new Section(no,new Section.Rect(divPoint, rect.right, rect.bottom, rect.left));
        _sections.Add(newSection);
        newSection._adjacentSection.Add(section._no);
        section._section.bottom = divPoint;
        section._adjacentSection.Add(no);

        return true;
    }
}
