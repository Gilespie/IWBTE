using System;
using System.IO;
using UnityEngine;

public class SaveSlot<T> where T : new()
{
    private readonly string _path;
    public T Data { get; private set; }

    public SaveSlot(string fileName)
    {
        string folder = Path.Combine(
             Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
             "GilespieCo",
             "Saves");

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        _path = Path.Combine(folder, fileName);
        Data = new T();
    }

    public bool Exists => File.Exists(_path);

    public void Save()
    {
        string json = JsonUtility.ToJson(Data, true);
        File.WriteAllText(_path, json);
    }

    public void Load()
    {
        Data = Exists
            ? JsonUtility.FromJson<T>(File.ReadAllText(_path))
            : new T();
    }
}