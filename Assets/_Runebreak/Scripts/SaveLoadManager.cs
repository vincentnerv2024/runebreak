using UnityEngine;
using System.IO;

/// <summary>
/// Handles save and load functionality using JSON serialization
/// </summary>
public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }
    
    private string saveFilePath;
    private const string SAVE_FILE_NAME = "gamesave.json";
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            saveFilePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void SaveGame(GameSaveData saveData)
    {
        try
        {
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(saveFilePath, json);
            Debug.Log($"Game saved to: {saveFilePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save game: {e.Message}");
        }
    }
    
    public GameSaveData LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.Log("No save file found.");
            return null;
        }
        
        try
        {
            string json = File.ReadAllText(saveFilePath);
            GameSaveData saveData = JsonUtility.FromJson<GameSaveData>(json);
            Debug.Log("Game loaded successfully.");
            return saveData;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load game: {e.Message}");
            return null;
        }
    }
    
    public bool SaveFileExists()
    {
        return File.Exists(saveFilePath);
    }
    
    public void DeleteSave()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("Save file deleted.");
        }
    }
}

[System.Serializable]
public class GameSaveData
{
    public GridData gridData;
    public ScoreData scoreData;
    public float playTime;
}

