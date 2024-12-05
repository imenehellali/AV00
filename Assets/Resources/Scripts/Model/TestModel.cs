using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TestModel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _txtmshPro;

    public async void StartModel()
    {
        Debug.Log("started generating model");
        /*Dictionary<string, float> outputFeatures = await PUWModel.Instance.CalculateoutputAttributes();
        
        foreach (var item in outputFeatures)
        {
            _txtmshPro.text += $"{item.Key}: {item.Value}\n";
        }
        string interpretation = await PsychologicalScoreInterpreter.Instance.GenerateInterpretationAsync(outputFeatures);
        _txtmshPro.text += interpretation;
        Debug.Log("finished interpretation");*/
        _txtmshPro.text += await OpenAIScoreInterpreter.Instance.GenerateInterpretation();
    }
    public void StopModel()
    {

    }
}
