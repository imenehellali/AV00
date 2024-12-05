using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class TMData : MonoBehaviour
{
    public static TMData Data { get; private set; }

    private void Awake()
    {
        if (Data == null)
        {
            Data = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        // _IDataService = new FileDataService(new JsonSerializer());

    }
    public void SaveData()
    {

    }


}