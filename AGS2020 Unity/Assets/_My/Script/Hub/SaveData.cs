using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveData : Singleton<SaveData>
{
    public PlayerData _playerData { get; private set; }

    /// <summary>
    /// セーブデータフォルダへのパス
    /// </summary>
    private string directoryPath;

    /// <summary>
    /// セーブデータファイルへのパス
    /// </summary>
    private string filePath;

    private new void Awake()
    {
        base.Awake();

        directoryPath = Path.Combine(Application.dataPath, @"SaveData");

        filePath = Path.Combine(directoryPath, "Data.txt");
    }

    // Start is called before the first frame update
    void Start()
    {
        if (_playerData == null)
        {
            _playerData = new PlayerData();
            CreateNewData();
        }
    }

    public void CreateNewData()
    {
        foreach(PlayerData.Status s in Enum.GetValues(typeof(PlayerData.Status)))
        {
            _playerData.upCounts[s] = 0;
        }

        _playerData.parts = 0;
    }

    public bool DirectoryCheck()
    {
        return Directory.Exists(directoryPath);
    }

    public bool FileCheck()
    {
        return File.Exists(filePath);
    }

    /// <summary>
    /// セーブする
    /// </summary>
    public void Save()
    {
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        //if (!File.Exists(filePath))
        //{
        //    File.Create(filePath);
        //}

        BinaryFormatter binaryFormatter = new BinaryFormatter();

        //書き込み専用でファイルを開く
        FileStream file = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write);

        try
        {
            //プレイヤデータをバイナリでシリアル化し
            //ファイルに書き込み
            binaryFormatter.Serialize(file, _playerData);
        }
        finally
        {
            //ファイルを閉じる
            file.Close();
        }
    }

    /// <summary>
    /// ロードする
    /// </summary>
    public void Load()
    {
        if (!Directory.Exists(directoryPath))
        {
            return;
        }

        if (!File.Exists(filePath))
        {
            return;
        }

        BinaryFormatter binaryFormatter = new BinaryFormatter();

        //読み取り専用でファイルを開く
        FileStream file = File.Open(filePath, FileMode.Open, FileAccess.Read);
        try
        {
            _playerData = (PlayerData)binaryFormatter.Deserialize(file);
        }
        finally
        {
            //ファイルを閉じる
            file.Close();
        }
    }
}

[System.Serializable]
public class PlayerData
{
    public enum Status
    {
        Hp,
        Cp,
        Atk,
        Def,
        Regene,
        BagCapacity
    }

    public Dictionary<Status, int> upCounts = new Dictionary<Status, int>();

    public int parts = 0;
}
