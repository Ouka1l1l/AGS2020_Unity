using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Section
{
    /// <summary>
    /// 短形
    /// </summary>
    public struct Rect
    {
        public int top;
        public int right;
        public int bottom;
        public int left;

        /// <summary>
        /// 横幅
        /// </summary>
        public int width
        {
            get { return (right - left) + 1; }
        }

        /// <summary>
        /// 縦幅
        /// </summary>
        public int height
        {
            get { return (bottom - top) + 1; }
        }

        public int Get()
        {
            return 1;
        }

        public int Area
        {
            get { return width * height; }
        }

        public Rect(int sTop, int sRight, int sBottom, int sLeft)
        {
            top = sTop;
            right = sRight;
            bottom = sBottom;
            left = sLeft;
        }
    }

    /// <summary>
    /// 区画番号
    /// </summary>
    public int _no { get; private set; }

    /// <summary>
    /// 区画
    /// </summary>
    public Rect _section;

    /// <summary>
    /// 部屋
    /// </summary>
    public Rect _room;

    /// <summary>
    /// 隣接区画
    /// </summary>
    public List<int> _adjacentSection { get; private set; }

    public Section(int no, Rect rect)
    {
        _no = no;
        _section = rect;
        _adjacentSection = new List<int>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateRoom(Vector2Int min, Vector2Int max, Vector2Int margin)
    {
        //部屋に使える広さ
        int width = _section.width - margin.x;
        int height = _section.height - margin.y;

        //部屋のサイズ
        int roomWidth = Random.Range(min.x, width + 1);
        int roomHeight = Random.Range(min.y, height + 1);

        if(roomWidth > max.x)
        {
            roomWidth = max.x;
        }

        if (roomHeight > max.y)
        {
            roomHeight = max.y;
        }

        width -= roomWidth;
        height -= roomHeight;

        int roomPosX = _section.left + Random.Range(0, width) + margin.x / 2;
        int roomPosY = _section.top + Random.Range(0, height) + margin.y / 2;

        _room = new Rect(roomPosY, roomPosX + roomWidth, roomPosY + roomHeight, roomPosX);
    }
}
