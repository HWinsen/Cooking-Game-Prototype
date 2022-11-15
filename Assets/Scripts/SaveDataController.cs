using Harryanto.CookingGame.LevelSelect;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveDataController : MonoBehaviour
{
    public static SaveDataController Instance;

    private const string key = "SaveData";

    private LevelScriptableObject[] _saveData;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        Load();
    }

    public void Save()
    {
        string json = JsonConvert.SerializeObject(_saveData, Formatting.Indented);
        //System.IO.File.WriteAllText(Application.persistentDataPath + "/savedata.json", json);
        PlayerPrefs.SetString(key, json);
    }

    public LevelScriptableObject[] Load()
    {
        if (PlayerPrefs.HasKey(key))
        {
            string json = PlayerPrefs.GetString(key);
            _saveData = JsonConvert.DeserializeObject<LevelScriptableObject[]>(json);
        }
        else
        {
            _saveData = Resources.LoadAll<LevelScriptableObject>("Level");
            Save();
        }
        return _saveData;
    }

    public void SetData(LevelScriptableObject[] data)
    {
        _saveData = data;
        Save();
    }
}
