using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// シングルトン化
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// シーンをまたいで生存させるか
    /// </summary>
    [SerializeField]
    private bool _dontDestroyOnLoad = false;

    public static T instance;

    protected void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            if (_dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
