using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class GameSettings : MonoBehaviour
{


    private string gameFilePath;
    public static GameSettings Instance { get; private set; }

    public UnityAction<string> OnSceneLoaded;
    public UnityAction<float> OnTimeUp;

    public float BetweenSceneDuration { get; private set; }
    public List<string> LevelSequence { get; private set; }
    public int CurrentLevelIndex { get; private set; }
    public List<float> LevelDurations { get; private set; }

    private float levelTimer;

    private float _PUWBGVolume;
    private float _PUWGMVolume;
    private float _PUWWaiterVolume;
    private float _LSHelpVolume;
    private float _LSWarnVolume;
    private float _GBFeedbackVolume;
    private float _TMFeedbackVolume;
    private float _TMBGVolume;


    private List<float> purchaseDurations = new List<float>();
    public void AddPurchaseDuration(float amount)
    {
        if (amount != 0f)
            purchaseDurations.Add(amount);
    }
    public List<float> GetPurchaseDurations() => purchaseDurations;
    public float GetPUWBGVolume() { return _PUWBGVolume; }
    public float GetPUWGMVolume() { return _PUWGMVolume; }
    public float GetPUWWaiterVolume() { return _PUWWaiterVolume; }
    public float GetLSHelpVolume() { return _LSHelpVolume; }
    public float GetLSWarnVolume() { return _LSWarnVolume; }
    public float GetGBFeedbackVolume() { return _GBFeedbackVolume; }
    public float GetTMFeedbackVolume() { return _TMFeedbackVolume; }
    public float GetTMBGVolume() { return _TMBGVolume; }

    private void Awake()
    {
        CurrentLevelIndex = 1;
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            gameFilePath = Path.Combine(Application.persistentDataPath, "GameData.json");
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        LoadGameSettings();
    }
    public void UpdateLevelSettings(List<string> newSequence, List<float> newLevelDuration, float newBetweenSceneDuration)
    {
        LevelSequence = new List<string>();
        LevelDurations = new List<float>();

        newSequence.ForEach(sequence => { LevelSequence.Add(sequence); });
        newLevelDuration.ForEach(levelDuration => { LevelDurations.Add(levelDuration); });

        BetweenSceneDuration = newBetweenSceneDuration;
        SaveGameSettings();
        TestSettings.UpdateText($"from game setting update {LevelSequence[CurrentLevelIndex - 1]}");
    }
    public void UpdateVolumeSettings(float PUWBGVolume, float PUWGMVolume, float PUWWaiterVolume, float lSHelpVolume, float lSWarnVolume, float GBFeedbackVolume, float TMFeedbackVolume, float TMBGVolume)
    {
        _PUWBGVolume = PUWBGVolume;
        _PUWGMVolume = PUWGMVolume;
        _PUWWaiterVolume = PUWWaiterVolume;
        _LSHelpVolume = lSHelpVolume;
        _LSWarnVolume = lSWarnVolume;
        _GBFeedbackVolume = GBFeedbackVolume;
        _TMFeedbackVolume = TMFeedbackVolume;
        _TMBGVolume = TMBGVolume;
    }
    private void SaveGameSettings()
    {
        GameData newGameData = new GameData
        {
            BetweenSceneDuration = BetweenSceneDuration,
            LevelSequence = LevelSequence,
            LevelDurations = LevelDurations,
            PUWBGVolume = _PUWBGVolume,
            PUWGMVolume = _PUWGMVolume,
            PUWWaiterVolume = _PUWWaiterVolume,
            LSHelpVolume = _LSHelpVolume,
            LSWarnVolume = _LSWarnVolume,
            GBFeedbackVolume = _GBFeedbackVolume,
            TMFeedbackVolume = _TMFeedbackVolume,
            TMBGVolume = _TMBGVolume,
        };
        try
        {
            // Convert the data object to a JSON string
            string jsonData = JsonConvert.SerializeObject(newGameData, Formatting.Indented);

            // Write the JSON string to the specified file path
            File.WriteAllText(gameFilePath, jsonData);

            Debug.Log("Settings data saved to: " + gameFilePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save settings data: " + e.Message);
        }
    }
    private void LoadGameSettings()
    {

    }

    public void StartLevelTimer()
    {
        levelTimer = LevelDurations[CurrentLevelIndex];
        StartCoroutine(LevelTimerCoroutine());
    }

    private IEnumerator LevelTimerCoroutine()
    {
        while (levelTimer > 0)
        {
            levelTimer -= Time.deltaTime;
            yield return null;
        }
        if (levelTimer <= 0)
        {
            //TODO check this is definetly wrong wtfff
            // Check if PUWScene is loaded before saving stats
            if (IsSceneLoaded("PUWScene"))
            {
                PUWStats.SaveStatsToParticipantData();
                MoneyManager.StoreMoneyInSafeAccount(CurrentLevelIndex - 1);

                NonRewardObject[] nonRewardObjects = FindObjectsOfType<NonRewardObject>();
                RewardObject[] rewardObjects = FindObjectsOfType<RewardObject>();

                foreach (var nonRewardObject in nonRewardObjects)
                {
                    nonRewardObject.UpdateTotalFixationTime();
                }
                foreach (var rewardObject in rewardObjects)
                {
                    rewardObject.UpdateTotalFixationTime();
                }
            }

            // Assuming levelIndex is 0-based and currentLevelIndex is 1-based

            OnTimeUp.Invoke(0);
        }

    }
    public bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName)
            {
                return true;
            }
        }
        return false;
    }
    public void LoadNextScene()
    {
        TestSettings.UpdateText($"from LoadScene index {CurrentLevelIndex} ");

        if (CurrentLevelIndex <= LevelSequence.Count)
        {
            TestSettings.UpdateText($"from if idx smaller");
            SceneLoaders.Instance.LoadLevel(LevelSequence[CurrentLevelIndex - 1]);
            if (CurrentLevelIndex >= 2) //not start scene
                SceneLoaders.Instance.UnloadCurrentScene(LevelSequence[CurrentLevelIndex - 2]);
            else //start scene
                SceneLoaders.Instance.UnloadCurrentScene("StartScene");
            SceneLoaders.Instance.LoadPUSScene();
            CurrentLevelIndex++;
            StartLevelTimer();
        }
        else
        {
            SceneLoaders.Instance.LoadLevel("EndScene");
        }
    }

    public void NotifySceneLoaded(string sceneName)
    {
        OnSceneLoaded.Invoke(sceneName);

        if (sceneName != "PUSScene")
        {
            InstructionPanel instructionPanel = FindObjectOfType<InstructionPanel>();
            if (instructionPanel != null)
            {
                instructionPanel.OnStartTask.AddListener(StartLevelTimer);
            }
        }
    }

}
