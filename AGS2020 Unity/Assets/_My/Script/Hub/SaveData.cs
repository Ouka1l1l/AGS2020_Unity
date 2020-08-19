using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveData : Singleton<SaveData>
{
    public PlayerData _playerData { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        _playerData = new PlayerData();
        CreateNewData();
    }

    public void CreateNewData()
    {
        foreach(PlayerData.Status s in Enum.GetValues(typeof(PlayerData.Status)))
        {
            _playerData.upCounts[s] = 0;
        }
    }

    /// <summary>
    /// セーブする
    /// </summary>
    /// <param name="playerData"></param>
    public void Save(PlayerData playerData)
    {
        var directoryPath = Path.Combine(Application.dataPath, @"SaveData");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        var filePath = Path.Combine(directoryPath, "Data.txt");
        if (!File.Exists(filePath))
        {
            File.Create(filePath);
        }

        BinaryFormatter binaryFormatter = new BinaryFormatter();

        //書き込み専用でファイルを開く
        FileStream file = File.Open(filePath, FileMode.Open, FileAccess.Write);

        try
        {
            //プレイヤデータをバイナリでシリアル化し
            //ファイルに書き込み
            binaryFormatter.Serialize(file, playerData);
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
        var directoryPath = Path.Combine(Application.dataPath, @"SaveData");
        if (!Directory.Exists(directoryPath))
        {
            return;
        }

        var filePath = Path.Combine(directoryPath, "Data.txt");
        if (!File.Exists(filePath))
        {
            return;
        }

        BinaryFormatter binaryFormatter = new BinaryFormatter();

        //読み取り専用でファイルを開く
        FileStream file = File.Open(filePath, FileMode.Open, FileAccess.Read);
        try
        {
            var t = (PlayerData)binaryFormatter.Deserialize(file);
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
