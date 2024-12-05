using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TestSettings : MonoBehaviour
{
     static TextMeshProUGUI textMeshProUGUI;

    private void Start()
    {
        textMeshProUGUI = gameObject.GetComponent<TextMeshProUGUI>();
    }

    public static void UpdateText(string text)
    {
        textMeshProUGUI.text += "   " + text;
    }

}
