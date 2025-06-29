using System.Runtime.Serialization.Formatters.Binary;
using GooglePlayGames.BasicApi.SavedGame;
using GooglePlayGames.BasicApi;
using GooglePlayGames;
using UnityEngine;
using System.IO;
using System;

public class CloudSaveManager : MonoBehaviour
{
    public static CloudSaveManager Instance { get; private set; }

    [SerializeField]
    private DataSource _dataSource;
    [SerializeField]
    private ConflictResolutionStrategy _conflicts;

    private string saveName = "save_0";

    private BinaryFormatter _formatter;

    public delegate void OnSaveLoadedDelegate();
    public event OnSaveLoadedDelegate OnSaveLoaded;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        _formatter = new BinaryFormatter();
    }

    // call this method whenever some value changes. Ex: CloudSaveManager.Instance.Save();
    public void Save()
    {
        if (Authentication.Authenticated)
            SaveToCloud();
    }

    public void Load()
    {
        if (Authentication.Authenticated)
            LoadFromCloud();
    }

    public void UseLocalData()
    {
        // use local device data
        // NOTE: SettingsManager uses local data by defult anyway

        if (OnSaveLoaded != null) // It is a MUST to check this, because the event is null if it has no subscribers
        {
            OnSaveLoaded();
        }
    }

    public void ApplyCloudData(SaveData.CloudSaveData data, bool dataExists)
    {
        if(!dataExists || data == null)
        {
            UseLocalData();
            return;
        }

        SettingsManager.instance.SaveSaveData(data);

        if (OnSaveLoaded != null) // It is a MUST to check this, because the event is null if it has no subscribers
        {
            OnSaveLoaded();
        }
    }

    private SaveData.CloudSaveData CollectAllData()
    {
        return SettingsManager.SaveData;
    }

    private void SaveToCloud()
    {
        OpenCloudSave(OnSaveResponse);
    }

    private void OnSaveResponse(SavedGameRequestStatus status, ISavedGameMetadata metadata)
    {
        if(status == SavedGameRequestStatus.Success)
        {
            var rawData = CollectAllData();
            if(rawData == null)
            {
                return;
            }

            var data = SerializeSaveData(rawData);
            if(data == null)
            {
                return;
            }

            var update = new SavedGameMetadataUpdate.Builder().Build();
            PlayGamesPlatform.Instance.SavedGame.CommitUpdate(metadata, update, data, SaveCallback);
        }
        else
        {
            Debug.LogError("OnSaveResponse error!");
        }
    }

    private void SaveCallback(SavedGameRequestStatus status, ISavedGameMetadata metadata)
    {
        if (status == SavedGameRequestStatus.Success)
            Debug.Log("Data saved successfully!");
        else
            Debug.Log("Data is not saved because of some error!");
    }

    private void LoadFromCloud()
    {
        OpenCloudSave(OnLoadResponse);
    }

    private void OnLoadResponse(SavedGameRequestStatus status, ISavedGameMetadata metadata)
    {
        if(status == SavedGameRequestStatus.Success)
        {
            PlayGamesPlatform.Instance.SavedGame.ReadBinaryData(metadata, LoadCallback);
        } 
        else
        {
            UseLocalData();
        }
    }

    private void LoadCallback(SavedGameRequestStatus status, byte[] data)
    {
        if(status == SavedGameRequestStatus.Success)
        {
            ApplyCloudData(DeserializeSaveData(data), data.Length > 0);
        }
        else
        {
            UseLocalData();
        }
    }

    private void OpenCloudSave(Action<SavedGameRequestStatus, ISavedGameMetadata> callback)
    {
        if (!PlayGamesPlatform.Instance.IsAuthenticated() 
            || string.IsNullOrEmpty(saveName)
        )
        {
            Debug.Log("OpenCloud Save Error!");
        }

        PlayGamesPlatform.Instance.SavedGame
            .OpenWithAutomaticConflictResolution(saveName, _dataSource, _conflicts, callback);
    }

    private byte[] SerializeSaveData(SaveData.CloudSaveData data)
    {
        try
        {
            using(MemoryStream ms = new MemoryStream())
            {
                _formatter.Serialize(ms, data);
                return ms.GetBuffer();
            }
        }
        catch(Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }

    private SaveData.CloudSaveData DeserializeSaveData(byte[] bytes)
    {
        if(bytes == null || bytes.Length == 0)
        {
            return null;
        }

        try
        {
            using(MemoryStream ms = new MemoryStream(bytes))
            {
                return (SaveData.CloudSaveData)_formatter.Deserialize(ms);
            }
        }
        catch(Exception e)
        {
            Debug.LogError(e);
            return null;
        }
    }
}
