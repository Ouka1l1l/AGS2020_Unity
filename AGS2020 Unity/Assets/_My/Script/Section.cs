using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Section
{
    public struct Rect
    {
        public int top;
        public int right;
        public int bottom;
        public int left;

        public int width
        {
            get { return right - left; }
        }

        public int height
        {
            get { return bottom - top; }
        }

        public Rect(int sTop, int sRight, int sBottom, int sLeft)
        {
            top = sTop;
            right = sRight;
            bottom = sBottom;
            left = sLeft;
        }
    }

    public Rect _rect;

    public Section(Rect rect)
    {
        _rect = rect;
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
