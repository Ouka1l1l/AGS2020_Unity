using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 区画クラス
/// </summary>
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

        /// <summary>
        /// 面積
        /// </summary>
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
    public Rect _sectionData;

    /// <summary>
    /// 部屋
    /// </summary>
    public Rect _roomData;

    /// <summary>
    /// 隣接区画
    /// </summary>
    public Dictionary<Dir,int> _adjacentSections { get; private set; }

    /// <summary>
    /// 隣接区画をセット
    /// </summary>
    /// <param name="key"></param> 隣接区画のある方向
    /// <param name="no"></param> 隣接区画の番号
    /// <returns></returns> 登録してあった区画番号　なかった場合は-1
    public int SetAdjacentSection(Dir key,int no)
    {
        if(_adjacentSections.ContainsKey(key))
        {
            int ret = _adjacentSections[key];
            _adjacentSections[key] = no;
            return ret;
        }
        else
        {
            _adjacentSections.Add(key, no);
            return -1;
        }
    }

    public Section(int no, Rect rect)
    {
        _no = no;
        _sectionData = rect;
        _adjacentSections = new Dictionary<Dir, int>();
    }

    /// <summary>
    /// 区画内に部屋を作る
    /// </summary>
    /// <param name="min"></param> 部屋の最小サイズ
    /// <param name="max"></param> 部屋の最大サイズ
    /// <param name="margin"></param> 区画端の余白
    public void CreateRoom(Vector2Int min, Vector2Int max, Vector2Int margin)
    {
        //部屋に使える広さ
        int width = _sectionData.width - margin.x;
        int height = _sectionData.height - margin.y;

        //部屋のサイズ
        int roomWidth = Random.Range(min.x, width + 1);
        int roomHeight = Random.Range(min.y, height + 1);

        //最大サイズを超えないように調整
        if(roomWidth > max.x)
        {
            roomWidth = max.x;
        }

        if (roomHeight > max.y)
        {
            roomHeight = max.y;
        }

        //部屋の位置をずらす
        width -= roomWidth;
        height -= roomHeight;

        int roomPosX = _sectionData.left + Random.Range(0, width) + margin.x / 2;
        int roomPosY = _sectionData.top + Random.Range(0, height) + margin.y / 2;

        _roomData = new Rect(roomPosY, roomPosX + roomWidth, roomPosY + roomHeight, roomPosX);
    }
}
