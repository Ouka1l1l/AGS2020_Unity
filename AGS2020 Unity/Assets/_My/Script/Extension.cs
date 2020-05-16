using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    public static class ColorExtension
{
    public static void SetAlpha(this Image Image, float a)
    {
        var color = Image.color;
        Image.color = new Color(color.r, color.g, color.b, a);
    }
}

public static class TransformExtension
{
    public static void SetX(this Transform transform, float x)
    {
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }
    public static void SetX(this Transform transform, int x)
    {
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }

    public static void SetY(this Transform transform, float y)
    {
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
    public static void SetY(this Transform transform, int y)
    {
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }

    public static void SetZ(this Transform transform, float z)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, z);
    }
    public static void SetZ(this Transform transform, int z)
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, z);
    }
}
