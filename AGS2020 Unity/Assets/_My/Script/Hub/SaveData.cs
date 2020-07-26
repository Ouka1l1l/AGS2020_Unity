using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    public PlayerData _playerData { get; private set; }

    // Start is called before the first frame update
    void Start()
    {

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
    public int hpUpCount = 0;
    public int cpUpCount = 0;
    public int atkUpCount = 0;
    public int defUpCount = 0;
    public int regeneUpCount = 0;
    public int itemMaxUpCount = 0;

    public int parts = 0;
}
