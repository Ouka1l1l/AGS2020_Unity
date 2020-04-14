using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public enum TerrainData
    {
        floor,
        Wall,
        Max
    }

    public TerrainData[,] _level { get; private set; } = new TerrainData[25, 50];

    private List<Section> _sections;

    private Vector2Int RoomMin = new Vector2Int(4,4);
    private Vector2Int RoomMax = new Vector2Int(25, 8);

    public Material material;

    private void Awake()
    {
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
                cube.transform.position = new Vector3(x, 0, -z);
                cube.transform.SetParent(transform);
                cube.GetComponent<Renderer>().material = material;

                if (_level[z, x] == TerrainData.Wall)
                {
                    var Wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    Wall.transform.position = new Vector3(x, 1, -z);
                    Wall.transform.SetParent(cube.transform);
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _sections.Add(new Section(new Section.Rect(0, _level.GetLength(1), _level.GetLength(0), 0)));
        SectionDivision();

        var section = _sections[1]._rect;
        for (int z = section.top; z < section.bottom; z++)
        {
            for (int x = section.left; x < section.right; x++)
            {
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.position = new Vector3(x, -5, -z);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public TerrainData GetTerrainData(int x,int z)
    {
        return _level[z, x];
    }

    private bool SectionDivision()
    {
        var rect = _sections[0]._rect;
        int divPointRight = rect.right - RoomMin.x;
        int divPointLeft = rect.left + RoomMin.x;

        int width = divPointRight - divPointLeft;
        width = Mathf.Min(width, RoomMax.x);

        int divPoint = Random.Range(0, width + 1);
        _sections.Add(new Section(new Section.Rect(rect.top, rect.right, rect.bottom, width)));
        _sections[0]._rect.right = width;

        return true;
    }
}
