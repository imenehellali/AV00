using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LifeSaverManager : MonoBehaviour
{
    private float levelDuration = 300f;
    private bool startUrgency = true;
    private float timeToStartUrgeny = 0f;
    private bool levelStarted = false;
    private int _rewardAmount = 200;

    [Header("Cases List")]
    [SerializeField]
    private List<GameObject> _casesObjs = new List<GameObject>();

    private Dictionary<string, Case> _cases = new Dictionary<string, Case>();
    public Dictionary<string, Case> GetCases() => _cases;
    private List<ResourceElement> _Resources = new List<ResourceElement>();

    [Header("Resource List")]
    [SerializeField]
    private TextMeshProUGUI _antidote;
    private int _antidoteAmount = 0;
    [SerializeField]
    private TextMeshProUGUI _air;
    private int _airAmount = 0;
    [SerializeField]
    private TextMeshProUGUI _water;
    private int _waterAmount = 0;


    [Header("Environmnet Variables")]
    [SerializeField]
    private MeshRenderer _envMaterial;
    [SerializeField]
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip _urgencyAudioClip;


    public bool AllowAntidoteConsumption() => _antidoteAmount > 0;
    public bool AllowAirConsumption() => _airAmount > 0;
    public bool AllowWaterConsumption() => _waterAmount > 0;

    //thsi shall update the count of how many a player has each resource left after consumption or regeneration
    private void UpdateResources(int amount, ResourceElement.Type type)
    {
        switch (type)
        {
            case ResourceElement.Type.Air:
                {
                    _airAmount = amount;
                    _air.text = _airAmount.ToString();
                    break;
                }
            case ResourceElement.Type.Water:
                {
                    _waterAmount = amount;
                    _water.text = _waterAmount.ToString();
                    break;
                }
            case ResourceElement.Type.Antidote:
                {
                    _antidoteAmount = amount;
                    _antidote.text = _antidoteAmount.ToString();
                    break;
                }
        }
    }
    public void ConsumeResource(GameObject obj)
    {
        ResourceElement.Type type = obj.GetComponent<ResourceElement>().type;
        _Resources.Find(x => x.type.Equals(type)).ConsumeResource();
    }

    public static LifeSaverManager Instance { get; private set; }

    private void OnEnable()
    {
        _Resources.ForEach(r => { r.AmountChanged += UpdateResources; });
    }
    private void OnDisable()
    {
        _Resources.ForEach(r => { r.AmountChanged -= UpdateResources; });
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        // _IDataService = new FileDataService(new JsonSerializer());

    }
    private void Start()
    {
        //TODO add receive level duration here 
        timeToStartUrgeny = levelDuration - 40f;
        InitCases();
        InitResources();
    }
    private void InitCases()
    {
        _cases.Add("CH1", _casesObjs[0].GetComponent<Case>().InitCase(false, true, 3, 1, 0, 90f)); //check time 1:30sec //CH1
        _cases.Add("CH2", _casesObjs[1].GetComponent<Case>().InitCase(false, true, 2, 0, 2, 120f)); //CH2 2min
        _cases.Add("CH3", _casesObjs[2].GetComponent<Case>().InitCase(false, true, 2, 0, 1, 180f)); //CH3 3min
        _cases.Add("CH4", _casesObjs[3].GetComponent<Case>().InitCase(false, false, 0, 3, 2, 210f)); //CH4 3:30

        _cases.Add("CA1", _casesObjs[4].GetComponent<Case>().InitCase(true, false, 4, 2, 0, 90f)); //check time 1:30sec //CA1
        _cases.Add("CA2", _casesObjs[5].GetComponent<Case>().InitCase(true, false, 1, 0, 2, 120f)); //CA2 2min
        _cases.Add("CA3", _casesObjs[6].GetComponent<Case>().InitCase(true, false, 1, 1, 1, 180f)); //CA3 3min
        _cases.Add("CA4", _casesObjs[7].GetComponent<Case>().InitCase(true, false, 0, 1, 4, 210f)); //CA4 3:30

        Debug.Log("finished initiating cases");
    }
    private void InitResources()
    {
        _Resources.Add(new ResourceElement(ResourceElement.Type.Air, ResourceElement.BelongTo.Individual, 2));
        _Resources.Add(new ResourceElement(ResourceElement.Type.Water, ResourceElement.BelongTo.Individual, 2));
        _Resources.Add(new ResourceElement(ResourceElement.Type.Antidote, ResourceElement.BelongTo.Individual, 5));

        Debug.Log("finished initiating resources");
    }

    private void Update()
    {
        if (levelStarted)
        {
            levelDuration -= Time.deltaTime;
            if (levelDuration <= timeToStartUrgeny)
                startUrgency = true;
        }
    }
    public void StartLevel()
    {
        levelStarted = true;
        foreach (Case _case in _cases.Values)
        {
            _case.Startcase();
        }
        StartCoroutine(Flicker());
    }

    //Functions to call from Game to behvae
    //
    //
    //
    //This will be called for every assignment for every resource indeoendantly 

    public void StopHelping(string CName)
    {
        if (levelDuration >= 30f)
            _cases[CName].StopHelping();
        else
            Debug.Log($"sorry too late to stop {CName} now");
    }


    //
    //
    //
    //
    private void AddRewardToMoneyManager()
    {
        float amount = 0f;
        bool stopped = true;
        foreach (var item in _cases.Values)
        {
            stopped &= item.StoppedHelping();
            amount += _rewardAmount * item.percentageDone;
        }

        //if psycho remove 80% of its game account money 
        if (stopped && amount == 0f)
        {
            amount = MoneyManager.GetGameAccount() * 0.8f * (-1f);
            MoneyManager.UpdateGameAccount(amount);
        }
        else if (stopped && amount > 0f) MoneyManager.UpdateMoney(amount);

        LSData.Data.SaveData();
        Debug.Log($"added  {amount}  to money   & saved Data");
    }

    private IEnumerator Flicker()
    {

        while (levelDuration > 0)
        {
            if (startUrgency)
            {
                _audioSource.PlayOneShot(_urgencyAudioClip);
                _envMaterial.material.EnableKeyword("_EMISSION");
                yield return new WaitForSeconds(_urgencyAudioClip.length);
                _envMaterial.material.DisableKeyword("_EMISSION");
            }

        }
        if (levelDuration < 0)
        {
            AddRewardToMoneyManager();
        }

    }
}
