using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class InstructionPanel : MonoBehaviour
{
    public UnityEvent OnStartTask = new UnityEvent();

    [SerializeField] private GameObject instructionPanel;
    [SerializeField] private InputActionReference Menu;

    //[SerializeField] private TextMeshProUGUI gameTranscript;
    [SerializeField] private Text gameTranscriptTextNormal;
    [SerializeField] private VideoPlayer gameVP;
    [SerializeField] private RenderTexture gameRT;

    //[SerializeField] private TextMeshProUGUI levelTranscript;
    [SerializeField] private Text levelTranscriptTextNormal;
    [SerializeField] private VideoPlayer levelVP;
    [SerializeField] private RawImage levelRawImage;

    [TextArea]
    public string gameTranscriptText;
    public VideoClip gameVideo;

    public List<string> levelTranscripts;
    public List<VideoClip> levelVideos;
    public RenderTexture[] renderTextures;

    private void OnDestroy()
    {
        Menu.action.started -= OpenInstrPanel;
    }

   
    private void Start()
    {
        instructionPanel.SetActive(true);
        Menu.action.started += OpenInstrPanel;

        DisplayGameInstruction();
        SetGameRenderTexture();
        if (IsSceneLoaded("StartScene"))
        {
            gameVP.Play();
        }

        else if (IsSceneLoaded("PUWScene"))
        {
            DisplayLevelInstruction(0);
            SetLevelRenderTexture(0); 
            StartCoroutine(StartLevelAfterPlay());
        }
        else if (IsSceneLoaded("LSScene"))
        {
            DisplayLevelInstruction(1);
            SetLevelRenderTexture(1);
            StartCoroutine(StartLevelAfterPlay()); 
        }
        else if (IsSceneLoaded("GBScene"))
        {
            DisplayLevelInstruction(2);
            SetLevelRenderTexture(2);
            StartCoroutine(StartLevelAfterPlay());
        }
        else if (IsSceneLoaded("TMScene"))
        {
            DisplayLevelInstruction(3);
            SetLevelRenderTexture(3);
            StartCoroutine(StartLevelAfterPlay());
        }
    }

    private void OpenInstrPanel(InputAction.CallbackContext callbackContext)
    {
        instructionPanel.SetActive(!instructionPanel.activeSelf);
    }

    private bool IsSceneLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName && scene.isLoaded)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator StartLevelAfterPlay()
    {
        levelVP.Play();
        yield return new WaitForSeconds(float.Parse(levelVP.clip.length.ToString()));
        instructionPanel.SetActive(false);
        FindAnyObjectByType<GhostBusterManager>().StartTask();
        //Add Thrill miner
        FindAnyObjectByType<LifeSaverManager>().StartLevel();
        FindAnyObjectByType<PopUpWerkManager>().StartLevel();
        TestEnv.Instance.StartLevelTimer();
    }
     public void DisplayGameInstruction()
    {
        if (!string.IsNullOrEmpty(gameTranscriptText))
        {
            //gameTranscript.text = gameTranscriptText;
            gameTranscriptTextNormal.text = gameTranscriptText; 
        }
    }

    public void SetGameRenderTexture()
    {
        if (gameRT != null)
        {
            gameVP.targetTexture = gameRT;
        }
    }

    public void DisplayLevelInstruction(int index)
    {
        if (levelTranscripts != null && index >= 0 && index < levelTranscripts.Count && !string.IsNullOrEmpty(levelTranscripts[index]))
        {
            //levelTranscript.text = levelTranscripts[index];
            levelTranscriptTextNormal.text = levelTranscripts[index].ToString();
        }
    }

    public void SetLevelRenderTexture(int index)
    {
        if (renderTextures != null && index >= 0 && index < renderTextures.Length && renderTextures[index] != null)
        {
            levelRawImage.texture = renderTextures[index];  // Set the RawImage's texture
            levelVP.targetTexture = renderTextures[index];  // Set the VideoPlayer's target texture
            levelVP.clip= levelVideos[index];
        }
    }
    public void StartTask()
    {
        instructionPanel.SetActive(false);
        OnStartTask.Invoke();
    }
}
