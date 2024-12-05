using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AllParticiipantDataManager : MonoBehaviour
{

    private string _path = "";
    private int _CGidx = 0;
    private int _ADidx = 0;
    public int getCGidx() => _CGidx;
    public int getADidx() => _ADidx;



    public static AllParticiipantDataManager Instance { get; private set; }

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _path= Path.Combine(Application.persistentDataPath, "Indeces.json");
            //Load data from Json file 

        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void IncCGidx()=>_CGidx++;
    public void IncADidx()=>_ADidx++;

    private void OnDestroy()
    {
        AllParticipantData _pdata = new AllParticipantData
        {
            CGidx = _CGidx,
            ADidx = _ADidx,
        };

        //Update the JsonFile
    }
}
