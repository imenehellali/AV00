using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestEnv : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI timer;
    [SerializeField] private TextMeshProUGUI moneyDisplay;
    [SerializeField] private GameObject instructionPanel;

    [Header("Level variables")]
    [SerializeField]
    private int CurrentLevelIndex = 0;
    [SerializeField]
    private float levelTimer= 300;
    private float OnTimeUP = 0f;


    private float remainingTime;
    public static TestEnv Instance { get; private set; }
    private void Awake()
    {
        CurrentLevelIndex = 1;
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
       MoneyManager.OnMoneyWon += UpdateMoneyDisplay;

    }
    private void OnDisable()
    {
       MoneyManager.OnMoneyWon -= UpdateMoneyDisplay;
    }
    private void Start()
    {
        instructionPanel.SetActive(true);
    }
    private void UpdateMoneyDisplay() => moneyDisplay.text = "\u20AC" + MoneyManager.GetMoney().ToString("N2");


    private void UpdateTimer(float time)
    {
        remainingTime = time;
        timer.text = FormatTime(remainingTime);
    }


    //Check this function ! i am pretty sure it does nothing .... 
    public void ToggleInstructionPanel()
    {
        instructionPanel.SetActive(!instructionPanel.activeSelf);
        if (!instructionPanel.activeSelf)
        {
            StartLevelTimer(); // Ensures timer continues without restarting
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60F);
        int seconds = Mathf.FloorToInt(time % 60F);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StartLevelTimer()
    {
        StartCoroutine(LevelTimerCoroutine());
    }

    private IEnumerator LevelTimerCoroutine()
    {
        while (levelTimer > 0)
        {
            levelTimer -= Time.deltaTime;
            UpdateTimer(levelTimer);
            yield return null;
        }
        if (levelTimer <= 0)
        {
            // Check if PUWScene is loaded before saving stats
            /*if (IsSceneLoaded("PUWScene"))
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
            }*/
            if (IsSceneLoaded("GBScene"))
            {
                LoadLevel("EndScene");
            }

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

    public void LoadLevel(string levelName)
    {
        StartCoroutine(LoadSceneAndNotify(levelName));
    }

    private IEnumerator LoadSceneAndNotify(string levelName)
    {
        UnloadCurrentScene("GBScene");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Single);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void UnloadCurrentScene(string levelName)
    {
        SceneManager.UnloadSceneAsync(levelName);
    }
}
