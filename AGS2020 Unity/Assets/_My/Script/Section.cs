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
            get { return right - left; }
        }

        /// <summary>
        /// 縦幅
        /// </summary>
        public int height
        {
            get { return bottom - top; }
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
}
